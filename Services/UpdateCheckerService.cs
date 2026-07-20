using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace UpGun_Mod_Tools_Launcher.Services
{
    public class UpdateInfo
    {
        public bool IsUpdateAvailable { get; set; }
        public Version CurrentVersion { get; set; } = new(1, 0, 0);
        public Version LatestVersion { get; set; } = new(1, 0, 0);
        public string ReleaseNotes { get; set; } = string.Empty;
        public string ReleasePageUrl { get; set; } = string.Empty;
        public string? DownloadUrl { get; set; }
        public string? AssetFileName { get; set; }
    }

    public class UpdateCheckerService
    {
        private readonly HttpClient _httpClient;

        public UpdateCheckerService(HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();

            // GitHub exige impérativement un header User-Agent
            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.UserAgent.Add(
                    new ProductInfoHeaderValue("UpGunModToolsLauncher", "1.0.0"));
            }
        }

        /// <summary>
        /// Vérifie si une mise à jour est disponible sur le dépôt GitHub.
        /// </summary>
        public async Task<UpdateInfo> CheckForUpdatesAsync(string owner, string repo, string currentVersionString)
        {
            var currentVersion = CleanAndParseVersion(currentVersionString);
            var url = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";

            try
            {
                using var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new UpdateInfo { IsUpdateAvailable = false, CurrentVersion = currentVersion };
                }

                using var stream = await response.Content.ReadAsStreamAsync();
                using var json = await JsonDocument.ParseAsync(stream);
                var root = json.RootElement;

                string tagName = root.GetProperty("tag_name").GetString() ?? "1.0.0";
                var latestVersion = CleanAndParseVersion(tagName);

                bool isNewer = latestVersion > currentVersion;

                string releaseNotes = root.TryGetProperty("body", out var bodyProp) ? bodyProp.GetString() ?? "" : "";
                string htmlUrl = root.TryGetProperty("html_url", out var urlProp) ? urlProp.GetString() ?? "" : "";

                // Sélection automatique de l'asset selon l'OS courant
                string? downloadUrl = null;
                string? assetName = null;

                if (root.TryGetProperty("assets", out var assetsProp) && assetsProp.ValueKind == JsonValueKind.Array)
                {
                    string targetKeyword = GetOsTargetKeyword();

                    foreach (var asset in assetsProp.EnumerateArray())
                    {
                        string name = asset.GetProperty("name").GetString() ?? "";
                        if (name.Contains(targetKeyword, StringComparison.OrdinalIgnoreCase) && name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadUrl = asset.GetProperty("browser_download_url").GetString();
                            assetName = name;
                            break;
                        }
                    }
                }

                return new UpdateInfo
                {
                    IsUpdateAvailable = isNewer,
                    CurrentVersion = currentVersion,
                    LatestVersion = latestVersion,
                    ReleaseNotes = releaseNotes,
                    ReleasePageUrl = htmlUrl,
                    DownloadUrl = downloadUrl,
                    AssetFileName = assetName
                };
            }
            catch
            {
                // En cas d'absence de réseau ou d'erreur API, on échoue silencieusement
                return new UpdateInfo { IsUpdateAvailable = false, CurrentVersion = currentVersion };
            }
        }

        /// <summary>
        /// Télécharge le fichier zip de la release avec rapport de progression (0.0 à 1.0 pour une ProgressBar).
        /// </summary>
        public async Task DownloadReleaseAsync(string downloadUrl, string destinationFilePath, IProgress<double>? progress = null)
        {
            using var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            var buffer = new byte[8192];
            long totalRead = 0;
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                totalRead += bytesRead;

                if (totalBytes > 0 && progress != null)
                {
                    progress.Report((double)totalRead / totalBytes);
                }
            }
        }

        private static Version CleanAndParseVersion(string versionString)
        {
            // Supprime 'v' ou 'V' et les suffixes de pré-release (ex: "v1.0.1-beta" -> "1.0.1")
            string cleaned = versionString.TrimStart('v', 'V').Split('-')[0];
            return Version.TryParse(cleaned, out var parsed) ? parsed : new Version(1, 0, 0);
        }

        private static string GetOsTargetKeyword()
        {
            if (OperatingSystem.IsWindows()) return "Windows";
            if (OperatingSystem.IsLinux()) return "Linux";
            if (OperatingSystem.IsMacOS()) return "macOS";
            return "Windows";
        }
    }
}