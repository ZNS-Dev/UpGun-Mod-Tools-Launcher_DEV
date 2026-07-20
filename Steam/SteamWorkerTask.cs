using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace UpGun_Mod_Tools_Launcher.Steam
{
    /// <summary>
    /// Logique Steamworks exécutée dans le process "worker" headless
    /// (lancé via --worker-query / --worker-publish). Aucune dépendance UI ici,
    /// donc ce fichier est 100% portable tel quel entre WinForms et Avalonia.
    /// </summary>
    public static class SteamWorkerTask
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetEnvironmentVariableWin(string lpName, string lpValue);

        private static void SetSteamAppIdEnv(string value)
        {
            // SetEnvironmentVariable via P/Invoke kernel32 n'existe que sous Windows.
            // On utilise l'API .NET standard, portable sur Linux/macOS.
            Environment.SetEnvironmentVariable("SteamAppId", value);
        }

        private static CallResult<SteamUGCQueryCompleted_t> m_SteamUGCQueryCompleted;
        private static CallResult<CreateItemResult_t> m_CreateItem;
        private static CallResult<SubmitItemUpdateResult_t> m_SubmitItemUpdate;

        private static UGCUpdateHandle_t m_CurrentUpdateHandle;
        private static bool enExecution = true;
        private static PublishedFileId_t fileIdTraite;

        public static void ExecuterRechercheWorkshop(string[] args)
        {
            if (args.Length < 2) return;

            uint appId = uint.Parse(args[1]);
            SetSteamAppIdEnv(appId.ToString());

            if (!SteamAPI.Init())
            {
                Console.WriteLine("ERROR:Steam n'est pas ouvert ! Veuillez démarrer Steam puis relancer l'application.");
                return;
            }

            m_SteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create((callback, bIOFailure) =>
            {
                if (!bIOFailure && callback.m_eResult == EResult.k_EResultOK)
                {
                    for (uint i = 0; i < callback.m_unNumResultsReturned; i++)
                    {
                        if (SteamUGC.GetQueryUGCResult(callback.m_handle, i, out SteamUGCDetails_t details))
                        {
                            string desc = details.m_rgchDescription.Replace("\r\n", "<BR>").Replace("\n", "<BR>");
                            // parts[4] = taille du fichier en octets, consommée par WorkshopItem.FileSize côté UI
                            Console.WriteLine($"{details.m_nPublishedFileId}|{details.m_rgchTitle}|{desc}|{details.m_rgchTags}|{details.m_nFileSize}");
                        }
                    }
                    SteamUGC.ReleaseQueryUGCRequest(callback.m_handle);
                }
                enExecution = false;
            });

            UGCQueryHandle_t handle = SteamUGC.CreateQueryUserUGCRequest(
                SteamUser.GetSteamID().GetAccountID(),
                EUserUGCList.k_EUserUGCList_Published,
                EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items,
                EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc,
                AppId_t.Invalid,
                new AppId_t(appId),
                1
            );

            SteamAPICall_t apiCall = SteamUGC.SendQueryUGCRequest(handle);
            m_SteamUGCQueryCompleted.Set(apiCall);

            while (enExecution)
            {
                SteamAPI.RunCallbacks();
                Thread.Sleep(50);
            }

            SteamAPI.Shutdown();
        }

        public static void ExecuterPublicationWorkshop(string[] args)
        {
            if (args.Length < 8) return;

            uint appId = uint.Parse(args[1]);
            fileIdTraite = new PublishedFileId_t(ulong.Parse(args[2]));
            string titre = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[3]));
            string desc = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[4]));
            string pakPath = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[5]));
            string iconPath = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[6]));
            string tags = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[7]));

            SetSteamAppIdEnv(appId.ToString());

            if (!SteamAPI.Init())
            {
                Console.WriteLine("ERROR:Steam n'est pas ouvert ! Veuillez démarrer Steam puis relancer l'application.");
                return;
            }

            m_CreateItem = CallResult<CreateItemResult_t>.Create((callback, bIOFailure) =>
            {
                if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
                {
                    Console.WriteLine("ERROR:Échec de création sur Steam. Code: " + callback.m_eResult);
                    enExecution = false;
                    return;
                }

                fileIdTraite = callback.m_nPublishedFileId;
                DemarrerUpdate(appId, fileIdTraite, titre, desc, pakPath, iconPath, tags);
            });

            m_SubmitItemUpdate = CallResult<SubmitItemUpdateResult_t>.Create((callback, bIOFailure) =>
            {
                if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
                {
                    Console.WriteLine("ERROR:Échec de l'envoi Steam. Code: " + callback.m_eResult);
                }
                else
                {
                    Console.WriteLine("SUCCESS:" + fileIdTraite.m_PublishedFileId);
                }
                enExecution = false;
            });

            if (fileIdTraite == PublishedFileId_t.Invalid)
            {
                Console.WriteLine("PROGRESS:Création du Mod sur Steam...");
                SteamAPICall_t apiCall = SteamUGC.CreateItem(new AppId_t(appId), EWorkshopFileType.k_EWorkshopFileTypeCommunity);
                m_CreateItem.Set(apiCall);
            }
            else
            {
                DemarrerUpdate(appId, fileIdTraite, titre, desc, pakPath, iconPath, tags);
            }

            while (enExecution)
            {
                SteamAPI.RunCallbacks();

                if (m_CurrentUpdateHandle != UGCUpdateHandle_t.Invalid)
                {
                    EItemUpdateStatus status = SteamUGC.GetItemUpdateProgress(m_CurrentUpdateHandle, out ulong bytesProcessed, out ulong bytesTotal);
                    string textStatus = "Préparation...";

                    switch (status)
                    {
                        case EItemUpdateStatus.k_EItemUpdateStatusPreparingConfig: textStatus = "Configuration..."; break;
                        case EItemUpdateStatus.k_EItemUpdateStatusUploadingContent: textStatus = "Envoi du contenu..."; break;
                        case EItemUpdateStatus.k_EItemUpdateStatusUploadingPreviewFile: textStatus = "Envoi de l'icône..."; break;
                        case EItemUpdateStatus.k_EItemUpdateStatusCommittingChanges: textStatus = "Validation..."; break;
                    }

                    if (bytesTotal > 0)
                    {
                        int progress = (int)((bytesProcessed * 100) / bytesTotal);
                        Console.WriteLine($"PROGRESS:{textStatus} ({progress}%)");
                    }
                    else
                    {
                        Console.WriteLine($"PROGRESS:{textStatus}");
                    }
                }

                Thread.Sleep(100);
            }

            SteamAPI.Shutdown();
        }

        private static void DemarrerUpdate(uint appId, PublishedFileId_t fileId, string titre, string desc, string pakPath, string iconPath, string tags)
        {
            AppId_t appIdObj = new AppId_t(appId);
            m_CurrentUpdateHandle = SteamUGC.StartItemUpdate(appIdObj, fileId);

            SteamUGC.SetItemTitle(m_CurrentUpdateHandle, titre);
            SteamUGC.SetItemDescription(m_CurrentUpdateHandle, desc);

            string dossierEnvoi = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DONT_DELETE");
            if (Directory.Exists(dossierEnvoi))
            {
                try { Directory.Delete(dossierEnvoi, true); } catch { }
            }
            Directory.CreateDirectory(dossierEnvoi);

            string nomPak = Path.GetFileName(pakPath);
            File.Copy(pakPath, Path.Combine(dossierEnvoi, nomPak), true);

            SteamUGC.SetItemContent(m_CurrentUpdateHandle, dossierEnvoi);

            if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
            {
                SteamUGC.SetItemPreview(m_CurrentUpdateHandle, iconPath);
            }

            if (!string.IsNullOrEmpty(tags))
            {
                List<string> listTags = tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                SteamUGC.SetItemTags(m_CurrentUpdateHandle, listTags);
            }

            SteamAPICall_t apiCall = SteamUGC.SubmitItemUpdate(m_CurrentUpdateHandle, "");
            m_SubmitItemUpdate.Set(apiCall);
        }
    }
}
