using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace UpGun_Mod_Tools_Launcher
{
    public static class SteamWorkerTask
    {
        private static CallResult<SteamUGCQueryCompleted_t> m_SteamUGCQueryCompleted;
        private static CallResult<CreateItemResult_t> m_CreateItem;
        private static CallResult<SubmitItemUpdateResult_t> m_SubmitItemUpdate;

        private static UGCUpdateHandle_t m_CurrentUpdateHandle;
        private static bool isRunning = true;
        private static PublishedFileId_t processedFileId;

        private static readonly string LogsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static readonly string LogFilePath = Path.Combine(LogsDirectory, "logs.txt");

        /// <summary>
        /// Writes a line to the console and, if it contains "ERROR:", also appends it to logs.txt.
        /// </summary>
        private static void WriteLine(string message)
        {
            Console.WriteLine(message);

            try
            {
                Directory.CreateDirectory(LogsDirectory);
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                File.AppendAllText(LogFilePath, $"[{timestamp}] {message}{Environment.NewLine}");
            }
            catch
            {
                // Silently ignore file write errors to avoid breaking the worker
            }
        }

        public static void ExecuteWorkshopSearch(string[] args)
        {
            WriteLine("=== Starting Steam Worker for Workshop Publish ===");
            if (args.Length < 2) return;
            WriteLine("Arguments received: " + string.Join(", ", args));

            isRunning = true;
            m_CurrentUpdateHandle = UGCUpdateHandle_t.Invalid;

            uint appId = uint.Parse(args[1]);
            Environment.SetEnvironmentVariable("SteamAppId", appId.ToString());

            try
            {
                if (!SteamAPI.Init())
                {
                    WriteLine("ERROR:Steam is not running! Please start Steam and restart the application.");
                    return;
                }
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
                string nativeLib = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "steam_api64.dll" : "libsteam_api.so";
                WriteLine($"ERROR:Failed to initialize Steam. Make sure Steam is running and {nativeLib} is present.");
                return;
            }

            WriteLine("Steam initialized.");
            WriteLine($"SteamID : {SteamUser.GetSteamID().m_SteamID}");
            WriteLine($"Persona : {SteamFriends.GetPersonaName()}");
            WriteLine($"AppID   : {SteamUtils.GetAppID().m_AppId}");

            m_SteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create((callback, bIOFailure) =>
            {
                WriteLine("===== QUERY CALLBACK =====");
                WriteLine($"IO Failure : {bIOFailure}");
                WriteLine($"Result     : {callback.m_eResult}");
                WriteLine($"Returned   : {callback.m_unNumResultsReturned}");
                WriteLine($"Total      : {callback.m_unTotalMatchingResults}");

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

                            WriteLine($"{details.m_nPublishedFileId}|{details.m_rgchTitle}|{desc}|{details.m_rgchTags}|{details.m_nFileSize}|{previewUrl}");
                        }
                    }
                    SteamUGC.ReleaseQueryUGCRequest(callback.m_handle);
                }
                isRunning = false;
            });

            WriteLine("Creating query...");
            UGCQueryHandle_t handle = SteamUGC.CreateQueryUserUGCRequest(
                SteamUser.GetSteamID().GetAccountID(),
                EUserUGCList.k_EUserUGCList_Published,
                EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items,
                EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc,
                new AppId_t(appId),   // <-- nCreatorAppID : l'AppID de l'application/outil
                AppId_t.Invalid,      // <-- nConsumerAppID : mis à Invalid pour lister les créations
                1
            );

            WriteLine($"Query Handle = {handle.m_UGCQueryHandle}");

            WriteLine("Sending query...");
            SteamAPICall_t apiCall = SteamUGC.SendQueryUGCRequest(handle);

            WriteLine($"API Call = {apiCall.m_SteamAPICall}");

            m_SteamUGCQueryCompleted.Set(apiCall);

            int ticks = 0;

            while (isRunning)
            {
                SteamAPI.RunCallbacks();

                if (++ticks % 20 == 0)
                    WriteLine("Waiting callback...");

                Thread.Sleep(50);
            }

            SteamAPI.Shutdown();
        }

        public static void ExecuteWorkshopPublish(string[] args)
        {
            WriteLine("=== Starting Steam Worker for Workshop Publish ===");
            if (args.Length < 8)
            {
                WriteLine("ERROR: Not enough arguments provided for publish worker.");
                return;
            }
            WriteLine("Arguments received: " + string.Join(", ", args));

            isRunning = true;
            m_CurrentUpdateHandle = UGCUpdateHandle_t.Invalid;

            try
            {
                uint appId = uint.Parse(args[1]);

                processedFileId = new PublishedFileId_t(ulong.Parse(args[2]));
                string title = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[3]));
                string desc = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[4]));
                string pakPath = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[5]));
                string iconPath = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[6]));
                string tags = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[7]));

                // IMPORTANT: Set SteamAppId BEFORE any SteamAPI calls
                Environment.SetEnvironmentVariable("SteamAppId", appId.ToString());

                try
                {
                    if (SteamAPI.RestartAppIfNecessary(new AppId_t(appId)))
                        return;

                    if (!SteamAPI.Init())
                    {
                        WriteLine("ERROR:Steam is not running! Please start Steam and restart the application.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    WriteLine(ex.ToString());
                    string nativeLib = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "steam_api64.dll" : "libsteam_api.so";
                    WriteLine($"ERROR:Failed to initialize Steam. Make sure Steam is running and {nativeLib} is present.");
                    return;
                }

                WriteLine("Steam initialized.");
                WriteLine($"SteamID : {SteamUser.GetSteamID().m_SteamID}");
                WriteLine($"Persona : {SteamFriends.GetPersonaName()}");
                WriteLine($"AppID   : {SteamUtils.GetAppID().m_AppId}");
                WriteLine($"OverlayEnabled: {SteamUtils.IsOverlayEnabled()}");
                WriteLine($"SubscribedApp: {SteamApps.BIsSubscribedApp(new AppId_t(appId))}");

                m_CreateItem = CallResult<CreateItemResult_t>.Create((callback, bIOFailure) =>
                {
                    if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
                    {
                        WriteLine("ERROR:Failed to create item on Steam. Code: " + callback.m_eResult);
                        isRunning = false;
                        return;
                    }

                    processedFileId = callback.m_nPublishedFileId;
                    StartUpdate(appId, processedFileId, title, desc, pakPath, iconPath, tags);
                });

                m_SubmitItemUpdate = CallResult<SubmitItemUpdateResult_t>.Create((callback, bIOFailure) =>
                {
                    WriteLine("==== Submit Callback ====");
                    WriteLine($"IO Failure : {bIOFailure}");
                    WriteLine($"Result     : {callback.m_eResult}");
                    WriteLine($"NeedsAgreement : {callback.m_bUserNeedsToAcceptWorkshopLegalAgreement}");
                    WriteLine("=========================");

                    if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
                    {
                        WriteLine("ERROR:Failed to upload to Steam. Code: " + callback.m_eResult);
                    }
                    else
                    {
                        WriteLine("SUCCESS:" + processedFileId.m_PublishedFileId);
                    }

                    isRunning = false;
                });

                WriteLine(processedFileId.m_PublishedFileId + "\n");
                WriteLine($"IsInvalid: {processedFileId == PublishedFileId_t.Invalid}");

                if (processedFileId.m_PublishedFileId == 0 || processedFileId == PublishedFileId_t.Invalid)
                {
                    WriteLine("PROGRESS:Creating Mod on Steam...");
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
                            WriteLine($"PROGRESS:{textStatus} ({progress}%)");
                        }
                        else
                        {
                            WriteLine($"PROGRESS:{textStatus}");
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
            WriteLine("========== StartUpdate ==========");
            WriteLine($"AppId      : {appId}");
            WriteLine($"FileId     : {fileId.m_PublishedFileId}");
            WriteLine($"Title      : {title}");
            WriteLine($"Description: {desc}");
            WriteLine($"PakPath    : {pakPath}");
            WriteLine($"IconPath   : {iconPath}");
            WriteLine($"Tags       : {tags}");

            AppId_t appIdObj = new AppId_t(appId);

            WriteLine("Calling StartItemUpdate...");
            m_CurrentUpdateHandle = SteamUGC.StartItemUpdate(appIdObj, fileId);
            WriteLine($"UpdateHandle = {m_CurrentUpdateHandle.m_UGCUpdateHandle}");

            WriteLine("SetItemTitle...");
            bool ok = SteamUGC.SetItemTitle(m_CurrentUpdateHandle, title);
            WriteLine($" -> {ok}");

            WriteLine("SetItemDescription...");
            ok = SteamUGC.SetItemDescription(m_CurrentUpdateHandle, desc);
            WriteLine($" -> {ok}");

            string uploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DONT_DELETE");

            WriteLine($"Upload folder = {uploadFolder}");

            if (Directory.Exists(uploadFolder))
            {
                WriteLine("Deleting previous upload folder...");
                try
                {
                    Directory.Delete(uploadFolder, true);
                }
                catch (Exception ex)
                {
                    WriteLine("Delete failed: " + ex);
                }
            }

            Directory.CreateDirectory(uploadFolder);

            WriteLine("Created upload folder.");

            WriteLine("Pak exists = " + File.Exists(pakPath));

            string pakFileName = Path.GetFileName(pakPath);
            string copiedFile = Path.Combine(uploadFolder, pakFileName);

            File.Copy(pakPath, copiedFile, true);

            WriteLine($"Copied pak to : {copiedFile}");
            WriteLine($"Copied exists : {File.Exists(copiedFile)}");

            WriteLine("SetItemContent...");
            ok = SteamUGC.SetItemContent(m_CurrentUpdateHandle, uploadFolder);
            WriteLine($" -> {ok}");

            if (!string.IsNullOrWhiteSpace(iconPath))
            {
                WriteLine($"Preview exists = {File.Exists(iconPath)}");

                if (File.Exists(iconPath))
                {
                    WriteLine("SetItemPreview...");
                    ok = SteamUGC.SetItemPreview(m_CurrentUpdateHandle, iconPath);
                    WriteLine($" -> {ok}");
                }
            }

            if (!string.IsNullOrWhiteSpace(tags))
            {
                List<string> tagList = tags
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();

                WriteLine("Tags:");
                foreach (string tag in tagList)
                    WriteLine(" - " + tag);

                WriteLine("SetItemTags...");
                ok = SteamUGC.SetItemTags(m_CurrentUpdateHandle, tagList);
                WriteLine($" -> {ok}");
            }

            WriteLine("Submitting update...");
            SteamAPICall_t apiCall = SteamUGC.SubmitItemUpdate(m_CurrentUpdateHandle, "");

            WriteLine($"SubmitItemUpdate Handle = {apiCall.m_SteamAPICall}");

            m_SubmitItemUpdate.Set(apiCall);

            WriteLine("========== End StartUpdate ==========");
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