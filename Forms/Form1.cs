using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpGun_Mod_Tools_Launcher.Forms;

namespace UpGun_Mod_Tools_Launcher
{
    public partial class Form1 : Form
    {
        private const uint TARGET_APP_ID = 1575870;

        public Form1()
        {
            InitializeComponent();

            this.Load -= Form1_Load;
            this.Load += Form1_Load;

            this.ListBoxWorkshopItem.SelectedIndexChanged -= ListBoxWorkshopItem_SelectedIndexChanged;
            this.ListBoxWorkshopItem.SelectedIndexChanged += ListBoxWorkshopItem_SelectedIndexChanged;

            this.BtnUpload.Click -= BtnUpload_Click;
            this.BtnUpload.Click += BtnUpload_Click;

            this.BtnRefreshList.Click -= BtnRefreshList_Click;
            this.BtnRefreshList.Click += BtnRefreshList_Click;

            this.Click -= Form1_Click;
            this.Click += Form1_Click;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            if (toolStripTextBox2 != null && string.IsNullOrWhiteSpace(toolStripTextBox2.Text))
            {
                Version current = Assembly.GetExecutingAssembly().GetName().Version;
                toolStripTextBox2.Text = $"v{current.Major}.{current.Minor}.{Math.Max(0, current.Build)}";
            }

            LoadSteamWorkshop();

            await CheckForUpdatesAsync();
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                string localText = toolStripTextBox2 != null ? toolStripTextBox2.Text : "";
                Version localVersion = ExtractVersion(localText);

                if (localVersion == null) return;

                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("UpGunLauncher");

                    string apiUrl = "https://api.github.com/repos/ZNS-Dev/UpGun-Mod-Tools-Launcher/releases/latest";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (!response.IsSuccessStatusCode) return;

                    string json = await response.Content.ReadAsStringAsync();

                    string remoteTag = ExtractJsonValue(json, "tag_name");
                    string remoteName = ExtractJsonValue(json, "name");
                    string downloadUrl = ExtractExeUrlFromAssets(json);

                    Version remoteVersion = ExtractVersion(remoteTag) ?? ExtractVersion(remoteName);

                    if (remoteVersion == null) return;

                    if (remoteVersion > localVersion)
                    {
                        DialogResult dialog = MessageBox.Show(
                            $"A new version is available!\n\n" +
                            $"• Current version: {localVersion}\n" +
                            $"• New version: {remoteVersion}\n\n" +
                            $"Would you like to download it and restart the application?",
                            "Update Available",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (dialog == DialogResult.Yes)
                        {
                            if (string.IsNullOrEmpty(downloadUrl))
                            {
                                MessageBox.Show("Error: No .exe file was found in the Release.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            await ReplaceAndRestartAsync(client, downloadUrl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Update Error] {ex.Message}");
            }
        }

        private async Task ReplaceAndRestartAsync(HttpClient client, string downloadUrl)
        {
            try
            {
                string currentExePath = Application.ExecutablePath;
                string tempExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update_temp.exe");

                byte[] exeData = await client.GetByteArrayAsync(downloadUrl);
                File.WriteAllBytes(tempExePath, exeData);

                string cmdArgs = $"/c timeout /t 2 /nobreak > nul & move /y \"{tempExePath}\" \"{currentExePath}\" & start \"\" \"{currentExePath}\"";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = cmdArgs,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = true
                };

                Process.Start(psi);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to update application: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Version ExtractVersion(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            Match match = Regex.Match(input, @"\d+(\.\d+)+");
            if (match.Success && Version.TryParse(match.Value, out Version parsed))
            {
                return parsed;
            }
            return null;
        }

        private string ExtractJsonValue(string json, string key)
        {
            Match match = Regex.Match(json, $"\"{key}\"\\s*:\\s*\"([^\"]+)\"");
            return match.Success ? match.Groups[1].Value : null;
        }

        private string ExtractExeUrlFromAssets(string json)
        {
            Match match = Regex.Match(json, @"\""browser_download_url\""\s*:\s*\""([^\""]+\.exe)\""", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : null;
        }

        private void BtnRefreshList_Click(object sender, EventArgs e) => LoadSteamWorkshop();
        private void BtnDiscordUG_Click(object sender, EventArgs e) => Process.Start("https://discord.com/invite/pMxHCVXJrz");
        private void BtnDiscordUGModding_Click(object sender, EventArgs e) => Process.Start("https://discord.gg/9VKrCEbyAV");
        private void ListBoxWorkshopItem_SelectedIndexChanged(object sender, EventArgs e) => UpdateButtonText();
        private void UpdateButtonText() => BtnUpload.Text = (ListBoxWorkshopItem.SelectedItem != null) ? "Update" : "Upload";

        private void Form1_Click(object sender, EventArgs e)
        {
            ListBoxWorkshopItem.SelectedIndex = -1;
        }

        private void LoadSteamWorkshop()
        {
            string steamErrorMessage = null;
            bool isGameMissing = false;

            try
            {
                ListBoxWorkshopItem.Items.Clear();

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    Arguments = $"--worker-query {TARGET_APP_ID}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    string line;
                    while ((line = process.StandardOutput.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        if (line.StartsWith("ERROR_NO_GAME:"))
                        {
                            steamErrorMessage = line.Replace("ERROR_NO_GAME:", "");
                            isGameMissing = true;
                            break;
                        }

                        if (line.StartsWith("ERROR:"))
                        {
                            steamErrorMessage = line.Replace("ERROR:", "");
                            break;
                        }

                        string[] parts = line.Split('|');
                        if (parts.Length >= 4)
                        {
                            if (ulong.TryParse(parts[0], out ulong rawFileId))
                            {
                                long.TryParse(parts.Length >= 5 ? parts[4] : "0", out long rawSize);
                                string previewUrl = parts.Length >= 6 ? parts[5] : "";

                                WorkshopItem item = new WorkshopItem
                                {
                                    FileId = new Steamworks.PublishedFileId_t(rawFileId),
                                    Title = parts[1],
                                    Description = parts[2].Replace("<BR>", "\n"),
                                    Tags = parts[3],
                                    FileSize = rawSize,
                                    PreviewUrl = previewUrl
                                };
                                ListBoxWorkshopItem.Items.Add(item);
                            }
                        }
                    }
                    process.WaitForExit(10000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving Workshop items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!string.IsNullOrEmpty(steamErrorMessage))
            {
                if (isGameMissing)
                {
                    MessageBox.Show(steamErrorMessage, "Game Not Owned", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    try
                    {
                        Process.Start($"steam://store/{TARGET_APP_ID}");
                    }
                    catch { }

                    Application.Exit();
                    return;
                }

                MessageBox.Show(steamErrorMessage, "Steam Not Detected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            ListBoxWorkshopItem.SelectedIndex = -1;
            UpdateButtonText();
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            Form2 form2;
            if (ListBoxWorkshopItem.SelectedItem is WorkshopItem selectedMod)
            {
                form2 = new Form2(TARGET_APP_ID, selectedMod.Title, selectedMod.Description, selectedMod.Tags, selectedMod.FileId, selectedMod.PreviewUrl);
            }
            else
            {
                form2 = new Form2(TARGET_APP_ID, "", "", "", Steamworks.PublishedFileId_t.Invalid, "");
            }

            if (form2.ShowDialog() == DialogResult.OK)
            {
                LoadSteamWorkshop();
            }

            ListBoxWorkshopItem.SelectedIndex = -1;
        }

        public class WorkshopItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Tags { get; set; }
            public long FileSize { get; set; }
            public string PreviewUrl { get; set; }
            public Steamworks.PublishedFileId_t FileId { get; set; }

            public string FormattedSize
            {
                get
                {
                    double bytes = FileSize;

                    if (bytes >= 1_000_000_000) return $"{bytes / 1_000_000_000.0:0.00} GB";
                    if (bytes >= 1_000_000) return $"{bytes / 1_000_000.0:0.00} MB";
                    if (bytes >= 1_000) return $"{bytes / 1_000.0:0.00} KB";

                    return $"{bytes} B";
                }
            }

            public override string ToString() => string.IsNullOrEmpty(Title) ? "NO TITLE!" : $"{Title} ({FormattedSize})";
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (WindowAbout aboutWindow = new WindowAbout())
                aboutWindow.ShowDialog();
        }
    }
}