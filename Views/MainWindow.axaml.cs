using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UpGun_Mod_Tools_Launcher.Models;
using UpGun_Mod_Tools_Launcher.Services;

namespace UpGun_Mod_Tools_Launcher.Views
{
    public partial class MainWindow : Window
    {
        private const uint APP_ID_CIBLE = 311210;
        // Lire la version dynamiquement, ou fallback à 1.0.0 si la version n'est pas disponible (ex: build en mode debug)
        private static readonly string VERSION_ACTUELLE =
    Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";

        private readonly ObservableCollection<WorkshopItem> _items = new();

        public MainWindow()
        {
            InitializeComponent();

            ListBoxWorkshopItem.ItemsSource = _items;
            TxtBoxVersion.Text = $"v{VERSION_ACTUELLE}";
            TxtAppId.Text = APP_ID_CIBLE.ToString();

            Opened += async (_, _) =>
            {
                // 1. Lance la vérification des MAJ en tâche de fond (non-bloquante)
                _ = VerifierMiseAJourAsync();

                // 2. Charge les mods du Workshop
                await ChargerWorkshopSteamAsync();
            };
        }

        private void TitleBarArea_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                BeginMoveDrag(e);
            }
        }

        private void BtnMinimize_Click(object? sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void BtnClose_Click(object? sender, RoutedEventArgs e) => Close();
        private void BtnDiscordMenu_Click(object? sender, RoutedEventArgs e) { /* ouvre le flyout déclaratif */ }

        private void BtnDiscordUG_Click(object? sender, RoutedEventArgs e) =>
            OuvrirUrl("https://discord.com/invite/pMxHCVXJrz");

        private void BtnDiscordUGModding_Click(object? sender, RoutedEventArgs e) =>
            OuvrirUrl("https://discord.gg/9VKrCEbyAV");

        private static void OuvrirUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
                try
                {
                    if (OperatingSystem.IsLinux()) Process.Start("xdg-open", url);
                    else if (OperatingSystem.IsMacOS()) Process.Start("open", url);
                }
                catch { /* on ignore silencieusement, ce n'est pas critique */ }
            }
        }

        private void ListBoxWorkshopItem_SelectionChanged(object? sender, SelectionChangedEventArgs e) =>
            MettreAJourTexteBouton();

        private void MettreAJourTexteBouton() =>
            BtnUpload.Content = (ListBoxWorkshopItem.SelectedItem != null) ? "Update" : "Upload";

        private async void BtnRefreshList_Click(object? sender, RoutedEventArgs e) =>
            await ChargerWorkshopSteamAsync();

        private async Task ChargerWorkshopSteamAsync()
        {
            BtnRefreshList.IsEnabled = false;
            TxtBoxMessage.Text = "Chargement de tes mods…";

            string? messageErreurSteam = null;
            var itemsCharges = new ObservableCollection<WorkshopItem>();

            try
            {
                await Task.Run(() =>
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = Environment.ProcessPath ?? Process.GetCurrentProcess().MainModule!.FileName,
                        Arguments = $"--worker-query {APP_ID_CIBLE}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };

                    using var process = Process.Start(startInfo);
                    if (process == null) return;

                    string? line;
                    while ((line = process.StandardOutput.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        if (line.StartsWith("ERROR:"))
                        {
                            messageErreurSteam = line.Replace("ERROR:", "");
                            break;
                        }

                        string[] parts = line.Split('|');
                        if (parts.Length >= 4 && ulong.TryParse(parts[0], out ulong rawFileId))
                        {
                            long.TryParse(parts.Length >= 5 ? parts[4] : "0", out long rawSize);

                            itemsCharges.Add(new WorkshopItem
                            {
                                FileId = new Steamworks.PublishedFileId_t(rawFileId),
                                Title = parts[1],
                                Description = parts[2].Replace("<BR>", "\n"),
                                Tags = parts[3],
                                FileSize = rawSize
                            });
                        }
                    }
                    process.WaitForExit(10000);
                });
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Erreur", "Erreur lors de la récupération : " + ex.Message);
            }

            if (!string.IsNullOrEmpty(messageErreurSteam))
            {
                await ShowErrorAsync("Steam non détecté", messageErreurSteam);
                Environment.Exit(1);
                return;
            }

            _items.Clear();
            foreach (var item in itemsCharges) _items.Add(item);

            EmptyState.IsVisible = _items.Count == 0;
            TxtBoxMessage.Text = _items.Count == 0
                ? ""
                : $"{_items.Count} mod(s) chargé(s) depuis le Workshop.";

            BtnRefreshList.IsEnabled = true;
            MettreAJourTexteBouton();
        }

        private async void BtnUpload_Click(object? sender, RoutedEventArgs e)
        {
            PublishWindow publishWindow = ListBoxWorkshopItem.SelectedItem is WorkshopItem modSelectionne
                ? new PublishWindow(APP_ID_CIBLE, modSelectionne.Title, modSelectionne.Description, modSelectionne.Tags, modSelectionne.FileId)
                : new PublishWindow(APP_ID_CIBLE, "", "", "", Steamworks.PublishedFileId_t.Invalid);

            var result = await publishWindow.ShowDialog<bool>(this);
            if (result)
            {
                await ChargerWorkshopSteamAsync();
            }
        }

        private async Task ShowErrorAsync(string title, string message)
        {
            var dialog = new MessageDialog(title, message);
            await dialog.ShowDialog(this);
        }

        private async Task VerifierMiseAJourAsync()
        {
            try
            {
                var checker = new UpdateCheckerService();

                Debug.WriteLine("[MAJ] Vérification de la mise à jour sur GitHub...");

                var updateInfo = await checker.CheckForUpdatesAsync("ZNS-Dev", "UpGun-Mod-Tools-Launcher_DEV", VERSION_ACTUELLE);

                // Affichage lisible dans la fenêtre de Sortie (Debug)
                Debug.WriteLine($"[MAJ] Résultat : IsAvailable={updateInfo.IsUpdateAvailable}, Current={updateInfo.CurrentVersion}, Latest={updateInfo.LatestVersion}, Url={updateInfo.DownloadUrl}");

                if (updateInfo.IsUpdateAvailable && !string.IsNullOrEmpty(updateInfo.DownloadUrl))
                {
                    TxtBoxMessage.Text = $"Nouvelle version v{updateInfo.LatestVersion} disponible !";

                    var progress = new Progress<double>(percent =>
                    {
                        TxtBoxMessage.Text = $"Téléchargement de la MAJ : {percent * 100:F0}%";
                    });

                    string destination = Path.Combine(AppContext.BaseDirectory, updateInfo.AssetFileName ?? "update.zip");

                    await checker.DownloadReleaseAsync(updateInfo.DownloadUrl, destination, progress);
                    TxtBoxMessage.Text = "Téléchargement terminé ! Extrais l'archive pour mettre à jour.";
                }
                else
                {
                    Debug.WriteLine("[MAJ] Aucune mise à jour disponible ou URL introuvable.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MAJ] Exception attrapée : {ex.Message}");
            }
        }
    }
}
