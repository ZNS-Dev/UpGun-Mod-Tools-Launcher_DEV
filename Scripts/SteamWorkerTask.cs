using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace UpGun_Mod_Tools_Launcher
{
    public static class SteamWorkerTask
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetEnvironmentVariable(string lpName, string lpValue);

        private static CallResult<SteamUGCQueryCompleted_t> m_SteamUGCQueryCompleted;
        private static CallResult<CreateItemResult_t> m_CreateItem;
        private static CallResult<SubmitItemUpdateResult_t> m_SubmitItemUpdate;

        private static UGCUpdateHandle_t m_CurrentUpdateHandle;
        private static bool isRunning = true;
        private static PublishedFileId_t processedFileId;

        public static void ExecuteWorkshopSearch(string[] args)
        {
            if (args.Length < 2) return;

            uint appId = uint.Parse(args[1]);
            SetEnvironmentVariable("SteamAppId", appId.ToString());

            if (!SteamAPI.Init())
            {
                Console.WriteLine("ERROR:Steam is not running! Please start Steam and restart the application.");
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

                            string previewUrl = "";
                            try
                            {
                                SteamUGC.GetQueryUGCPreviewURL(callback.m_handle, i, out previewUrl, 1024);
                            }
                            catch { }

                            Console.WriteLine($"{details.m_nPublishedFileId}|{details.m_rgchTitle}|{desc}|{details.m_rgchTags}|{details.m_nFileSize}|{previewUrl}");
                        }
                    }
                    SteamUGC.ReleaseQueryUGCRequest(callback.m_handle);
                }
                isRunning = false;
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

            while (isRunning)
            {
                SteamAPI.RunCallbacks();
                Thread.Sleep(50);
            }

            SteamAPI.Shutdown();
        }

        public static void ExecuteWorkshopPublish(string[] args)
        {
            if (args.Length < 8) return;

            try
            {
                uint appId = uint.Parse(args[1]);
                processedFileId = new PublishedFileId_t(ulong.Parse(args[2]));
                string title = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[3]));
                string desc = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[4]));
                string pakPath = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[5]));
                string iconPath = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[6]));
                string tags = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[7]));

                SetEnvironmentVariable("SteamAppId", appId.ToString());

                if (!SteamAPI.Init())
                {
                    Console.WriteLine("ERROR:Steam is not running! Please start Steam and restart the application.");
                    return;
                }

                m_CreateItem = CallResult<CreateItemResult_t>.Create((callback, bIOFailure) =>
                {
                    if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
                    {
                        Console.WriteLine("ERROR:Failed to create item on Steam. Code: " + callback.m_eResult);
                        isRunning = false;
                        return;
                    }

                    processedFileId = callback.m_nPublishedFileId;
                    StartUpdate(appId, processedFileId, title, desc, pakPath, iconPath, tags);
                });

                m_SubmitItemUpdate = CallResult<SubmitItemUpdateResult_t>.Create((callback, bIOFailure) =>
                {
                    if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
                    {
                        Console.WriteLine("ERROR:Failed to upload to Steam. Code: " + callback.m_eResult);
                    }
                    else
                    {
                        Console.WriteLine("SUCCESS:" + processedFileId.m_PublishedFileId);
                    }
                    isRunning = false;
                });

                if (processedFileId == PublishedFileId_t.Invalid)
                {
                    Console.WriteLine("PROGRESS:Creating Mod on Steam...");
                    SteamAPICall_t apiCall = SteamUGC.CreateItem(new AppId_t(appId), EWorkshopFileType.k_EWorkshopFileTypeCommunity);
                    m_CreateItem.Set(apiCall);
                }
                else
                {
                    StartUpdate(appId, processedFileId, title, desc, pakPath, iconPath, tags);
                }

                while (isRunning)
                {
                    SteamAPI.RunCallbacks();

                    if (m_CurrentUpdateHandle != UGCUpdateHandle_t.Invalid)
                    {
                        EItemUpdateStatus status = SteamUGC.GetItemUpdateProgress(m_CurrentUpdateHandle, out ulong bytesProcessed, out ulong bytesTotal);
                        string textStatus = "Preparing...";

                        switch (status)
                        {
                            case EItemUpdateStatus.k_EItemUpdateStatusPreparingConfig: textStatus = "Configuring..."; break;
                            case EItemUpdateStatus.k_EItemUpdateStatusUploadingContent: textStatus = "Uploading content..."; break;
                            case EItemUpdateStatus.k_EItemUpdateStatusUploadingPreviewFile: textStatus = "Uploading preview icon..."; break;
                            case EItemUpdateStatus.k_EItemUpdateStatusCommittingChanges: textStatus = "Committing changes..."; break;
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
            finally
            {
                CleanTemporaryFolder();
            }
        }

        private static void StartUpdate(uint appId, PublishedFileId_t fileId, string title, string desc, string pakPath, string iconPath, string tags)
        {
            AppId_t appIdObj = new AppId_t(appId);
            m_CurrentUpdateHandle = SteamUGC.StartItemUpdate(appIdObj, fileId);

            SteamUGC.SetItemTitle(m_CurrentUpdateHandle, title);
            SteamUGC.SetItemDescription(m_CurrentUpdateHandle, desc);

            string uploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DONT_DELETE");
            if (Directory.Exists(uploadFolder))
            {
                try { Directory.Delete(uploadFolder, true); } catch { }
            }
            Directory.CreateDirectory(uploadFolder);

            string pakFileName = Path.GetFileName(pakPath);
            File.Copy(pakPath, Path.Combine(uploadFolder, pakFileName), true);

            SteamUGC.SetItemContent(m_CurrentUpdateHandle, uploadFolder);

            if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
            {
                SteamUGC.SetItemPreview(m_CurrentUpdateHandle, iconPath);
            }

            if (!string.IsNullOrEmpty(tags))
            {
                List<string> tagList = tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                SteamUGC.SetItemTags(m_CurrentUpdateHandle, tagList);
            }

            SteamAPICall_t apiCall = SteamUGC.SubmitItemUpdate(m_CurrentUpdateHandle, "");
            m_SubmitItemUpdate.Set(apiCall);
        }

        private static void CleanTemporaryFolder()
        {
            string uploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DONT_DELETE");
            if (Directory.Exists(uploadFolder))
            {
                try { Directory.Delete(uploadFolder, true); } catch { }
            }
        }
    }
}