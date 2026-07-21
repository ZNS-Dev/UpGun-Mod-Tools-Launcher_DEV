using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UpGun_Mod_Tools_Launcher.Forms;

namespace UpGun_Mod_Tools_Launcher
{
    public partial class MainWindow : Window
    {
        // TODO: Put Upgun AppId here when possible, for now it's UploadLabs' one
        private const uint TARGET_APP_ID = 3606890;

        public MainWindow()
        {
            InitializeComponent();
            this.Opened += async (s, e) => await LoadSteamWorkshop();
            Debug.WriteLine("MainWindow initialized.");
        }

        private void ListBoxWorkshopItem_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            UpdateButtonText();
        }

        private void UpdateButtonText()
        {
            BtnUpload.Content = (ListBoxWorkshopItem.SelectedItem != null) ? "Update" : "Upload";
        }

        private void BtnRefreshList_Click(object? sender, RoutedEventArgs e) => LoadSteamWorkshop();

        private void BtnDiscordUG_Click(object? sender, RoutedEventArgs e) => OpenUrl("https://discord.com/invite/pMxHCVXJrz");
        
        private void BtnDiscordUGModding_Click(object? sender, RoutedEventArgs e) => OpenUrl("https://discord.gg/9VKrCEbyAV");

        private void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private async Task LoadSteamWorkshop()
        {
            Debug.WriteLine("=== Début de LoadSteamWorkshop ===");
            string? steamErrorMessage = null;

            try
            {
                ListBoxWorkshopItem.Items.Clear();

                string? exePath = Environment.ProcessPath;
                if (string.IsNullOrEmpty(exePath)) return;

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = $"--worker-query {TARGET_APP_ID}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true, // On capture aussi les erreurs au cas où
                    CreateNoWindow = true
                };

                using (Process? process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        // Lecture de la sortie standard
                        string? line;
                        while ((line = process.StandardOutput.ReadLine()) != null)
                        {
                            Debug.WriteLine($"[WORKER OUT] {line}"); // <-- Regarde ça dans la fenêtre de sortie VS !

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

                        // Lecture de la sortie d'erreur standard (au cas où SteamAPI écrit dedans)
                        string? errLine = process.StandardError.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(errLine))
                        {
                            Debug.WriteLine($"[WORKER ERR] {errLine}");
                            if (errLine.Contains("ERROR:"))
                            {
                                steamErrorMessage = errLine.Replace("ERROR:", "").Trim();
                            }
                        }

                        process.WaitForExit(10000);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error retrieving items: " + ex.Message);
            }

            Debug.WriteLine($"=== Fin du worker. steamErrorMessage = '{steamErrorMessage}' ===");

            // Si Steam n'est pas détecté, on affiche le pop-up d'erreur puis on ferme l'application
            if (!string.IsNullOrEmpty(steamErrorMessage))
            {
                Debug.WriteLine("Affichage du pop-up d'erreur Steam...");
                await MessageBox.Show(this, "Steam non détecté", steamErrorMessage);
                Close();
                return;
            }

            ListBoxWorkshopItem.SelectedIndex = -1;
            UpdateButtonText();
        }

        private async void BtnUpload_Click(object? sender, RoutedEventArgs e)
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

            await form2.ShowDialog(this);
            LoadSteamWorkshop();
        }

        private async void HelpToolStripMenuItem_Click(object? sender, RoutedEventArgs e)
        {
            var aboutWindow = new WindowAbout();
            await aboutWindow.ShowDialog(this);
        }

        public class WorkshopItem
        {
            public string Title { get; set; } = "";
            public string Description { get; set; } = "";
            public string Tags { get; set; } = "";
            public long FileSize { get; set; }
            public string PreviewUrl { get; set; } = "";
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
    }
}