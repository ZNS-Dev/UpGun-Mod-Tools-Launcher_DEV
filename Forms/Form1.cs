using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace UpGun_Mod_Tools_Launcher
{
    public partial class Form1 : Form
    {
        private const uint APP_ID_CIBLE = 311210;

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
        }

        private void Form1_Load(object sender, EventArgs e) => ChargerWorkshopSteam();
        private void BtnRefreshList_Click(object sender, EventArgs e) => ChargerWorkshopSteam();
        private void BtnDiscordUG_Click(object sender, EventArgs e) => Process.Start("https://discord.com/invite/pMxHCVXJrz");
        private void BtnDiscordUGModding_Click(object sender, EventArgs e) => Process.Start("https://discord.gg/9VKrCEbyAV");
        private void ListBoxWorkshopItem_SelectedIndexChanged(object sender, EventArgs e) => MettreAJourTexteBouton();
        private void MettreAJourTexteBouton() => BtnUpload.Text = (ListBoxWorkshopItem.SelectedItem != null) ? "Update" : "Upload";

        private void ChargerWorkshopSteam()
        {
            string messageErreurSteam = null;

            try
            {
                ListBoxWorkshopItem.Items.Clear();

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    Arguments = $"--worker-query {APP_ID_CIBLE}",
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
                            messageErreurSteam = line.Replace("ERROR:", "");
                            break;
                        }

                        string[] parts = line.Split('|');
                        if (parts.Length >= 4)
                        {
                            if (ulong.TryParse(parts[0], out ulong rawFileId))
                            {
                                long.TryParse(parts.Length >= 5 ? parts[4] : "0", out long rawSize);

                                WorkshopItem item = new WorkshopItem
                                {
                                    FileId = new Steamworks.PublishedFileId_t(rawFileId),
                                    Title = parts[1],
                                    Description = parts[2].Replace("<BR>", "\n"),
                                    Tags = parts[3],
                                    FileSize = rawSize
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

            if (!string.IsNullOrEmpty(messageErreurSteam))
            {
                MessageBox.Show(messageErreurSteam, "Steam Not Detected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            MettreAJourTexteBouton();
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            Form2 form2;
            if (ListBoxWorkshopItem.SelectedItem is WorkshopItem modSelectionne)
            {
                form2 = new Form2(APP_ID_CIBLE, modSelectionne.Title, modSelectionne.Description, modSelectionne.Tags, modSelectionne.FileId);
            }
            else
            {
                form2 = new Form2(APP_ID_CIBLE, "", "", "", Steamworks.PublishedFileId_t.Invalid);
            }

            if (form2.ShowDialog() == DialogResult.OK)
            {
                ChargerWorkshopSteam();
            }
        }

        public class WorkshopItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Tags { get; set; }
            public long FileSize { get; set; }
            public Steamworks.PublishedFileId_t FileId { get; set; }

            public string FormattedSize
            {
                get
                {
                    double bytes = FileSize;
                    if (bytes >= 1000000000) return $"{bytes / 1000000000.0:0.00} GB";
                    if (bytes >= 1000000) return $"{bytes / 1000000.0:0.00} MB";
                    if (bytes >= 1000) return $"{bytes / 1000.0:0.00} KB";
                    return $"{bytes} B";
                }
            }

            public override string ToString() => string.IsNullOrEmpty(Title) ? "NO TITLE!" : $"{Title} ({FormattedSize})";
        }
    }
}