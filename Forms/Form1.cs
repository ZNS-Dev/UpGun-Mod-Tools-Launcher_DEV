using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpGun_Mod_Tools_Launcher.Forms;

namespace UpGun_Mod_Tools_Launcher
{
    public partial class Form1 : Form
    {
        private const uint TARGET_APP_ID = 311210;

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
            LoadSteamWorkshop();
            await CheckForUpdatesAsync();
        }

        /// <summary>
        /// Récupère la dernière release sur GitHub et affiche son nom/titre dans toolStripTextBox2.
        /// </summary>
        private async Task CheckForUpdatesAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // L'API GitHub requiert un header User-Agent
                    client.DefaultRequestHeaders.Add("User-Agent", "UpGunModToolsLauncher");

                    string url = "https://api.github.com/repos/ZNS-Dev/UpGun-Mod-Tools-Launcher/releases/latest";
                    string json = await client.GetStringAsync(url);

                    // Extraction de la propriété "name" (Nom de la release)
                    int nameIndex = json.IndexOf("\"name\"");
                    if (nameIndex != -1)
                    {
                        int startQuote = json.IndexOf('"', json.IndexOf(':', nameIndex) + 1);
                        int endQuote = json.IndexOf('"', startQuote + 1);

                        if (startQuote != -1 && endQuote != -1)
                        {
                            string releaseName = json.Substring(startQuote + 1, endQuote - startQuote - 1);
                            toolStripTextBox2.Text = releaseName;
                        }
                    }
                }
            }
            catch
            {
                // Valeur par défaut en cas d'erreur réseau
                toolStripTextBox2.Text = "v1.0.0";
            }
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
                MessageBox.Show("Error retrieving items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!string.IsNullOrEmpty(steamErrorMessage))
            {
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

                    if (bytes >= 1_000_000_000)
                        return $"{bytes / 1_000_000_000.0:0.00} GB";

                    if (bytes >= 1_000_000)
                        return $"{bytes / 1_000_000.0:0.00} MB";

                    if (bytes >= 1_000)
                        return $"{bytes / 1_000.0:0.00} KB";

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