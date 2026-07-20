using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UpGun_Mod_Tools_Launcher.Steam;

namespace UpGun_Mod_Tools_Launcher.Views
{
    public partial class PublishWindow : Window
    {
        private readonly uint m_AppIdCible;
        private readonly PublishedFileId_t m_FileId;
        private readonly bool estUneCreation;
        private string m_PakPath = "";
        private string m_IconPath = "";
        private Process? workerProcess;

        // Constructeur pour le designer XAML uniquement
        public PublishWindow()
        {
            InitializeComponent();
        }

        public PublishWindow(uint appIdCible, string titreMod, string descriptionMod, string tagsMod, PublishedFileId_t fileId) : this()
        {
            m_AppIdCible = appIdCible;
            m_FileId = fileId;
            estUneCreation = (m_FileId == PublishedFileId_t.Invalid);

            TxtTitle.Text = titreMod;
            TxtDescription.Text = descriptionMod;

            TxtWindowTitle.Text = estUneCreation ? "Publish Mod" : "Update Mod";
            BtnPublishMod.Content = estUneCreation ? "Publish Mod" : "Update Mod";

            if (!string.IsNullOrEmpty(tagsMod))
            {
                var listeTags = tagsMod.Split(',').Select(t => t.Trim()).ToArray();
                foreach (var child in TagsPanel.Children)
                {
                    if (child is ToggleButton tb && tb.Content is string tagName &&
                        listeTags.Contains(tagName, StringComparer.OrdinalIgnoreCase))
                    {
                        tb.IsChecked = true;
                    }
                }
            }
        }

        // ===================== Drag / fermeture =====================
        private void DragArea_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) BeginMoveDrag(e);
        }

        private void BtnCloseWindowPublish_Click(object? sender, RoutedEventArgs e)
        {
            Close(false); // Window.Close(object? dialogResult) natif d'Avalonia
        }

        // ===================== Sélection du .pak =====================
        private async void DropZonePak_PointerPressed(object? sender, PointerPressedEventArgs e) =>
            await ChoisirFichierPak();

        private async System.Threading.Tasks.Task ChoisirFichierPak()
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Choisir un fichier .pak",
                AllowMultiple = false,
                FileTypeFilter = new[] { new FilePickerFileType("Pak file") { Patterns = new[] { "*.pak" } } }
            });

            if (files.Count > 0)
            {
                DefinirPak(files[0].Path.LocalPath);
            }
        }

        private void DropZonePak_DragOver(object? sender, DragEventArgs e)
        {
            bool valide = e.Data.Contains(DataFormats.Files) &&
                          e.Data.GetFiles()?.Any(f => f.Path.LocalPath.EndsWith(".pak", StringComparison.OrdinalIgnoreCase)) == true;
            e.DragEffects = valide ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void DropZonePak_Drop(object? sender, DragEventArgs e)
        {
            var fichier = e.Data.GetFiles()?.FirstOrDefault(f => f.Path.LocalPath.EndsWith(".pak", StringComparison.OrdinalIgnoreCase));
            if (fichier != null)
            {
                DefinirPak(fichier.Path.LocalPath);
            }
        }

        private void DefinirPak(string path)
        {
            m_PakPath = path;
            TxtPakFileName.Text = Path.GetFileName(path);
        }

        // ===================== Sélection de l'icône =====================
        private async void ChoisirIcone_Click(object? sender, PointerPressedEventArgs e)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Choisir une icône",
                AllowMultiple = false,
                FileTypeFilter = new[] { new FilePickerFileType("Icon file") { Patterns = new[] { "*.png" } } }
            });

            if (files.Count > 0)
            {
                m_IconPath = files[0].Path.LocalPath;
                TxtIconPath.Text = Path.GetFileName(m_IconPath);
                try
                {
                    IconPreview.Source = new Bitmap(m_IconPath);
                }
                catch { /* aperçu non critique */ }
            }
        }

        // ===================== Publication =====================
        private void BtnPublishMod_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(m_PakPath) || !File.Exists(m_PakPath) || Path.GetExtension(m_PakPath).ToLower() != ".pak")
            {
                _ = new MessageDialog("Erreur", "Fichier .pak invalide !").ShowDialog(this);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtTitle.Text))
            {
                _ = new MessageDialog("Titre manquant", "Veuillez indiquer un titre.").ShowDialog(this);
                return;
            }

            var tagsCoches = TagsPanel.Children
                .OfType<ToggleButton>()
                .Where(tb => tb.IsChecked == true)
                .Select(tb => tb.Content?.ToString() ?? "")
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList();

            GererEtatInterface(false);
            ProgressSection.IsVisible = true;

            string titreB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(TxtTitle.Text.Trim()));
            string descB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(TxtDescription.Text?.Trim() ?? ""));
            string pakB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(m_PakPath));
            string iconB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(m_IconPath ?? ""));
            string tagsB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(string.Join(",", tagsCoches)));

            var startInfo = new ProcessStartInfo
            {
                FileName = Environment.ProcessPath ?? Process.GetCurrentProcess().MainModule!.FileName,
                Arguments = $"--worker-publish {m_AppIdCible} {m_FileId.m_PublishedFileId} \"{titreB64}\" \"{descB64}\" \"{pakB64}\" \"{iconB64}\" \"{tagsB64}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            workerProcess = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

            workerProcess.OutputDataReceived += (s, argsData) =>
            {
                if (string.IsNullOrEmpty(argsData.Data)) return;

                Dispatcher.UIThread.Post(() =>
                {
                    if (argsData.Data.StartsWith("PROGRESS:"))
                    {
                        string texte = argsData.Data.Replace("PROGRESS:", "");
                        TxtProgressLabel.Text = texte;

                        var match = System.Text.RegularExpressions.Regex.Match(texte, @"\((\d+)%\)");
                        if (match.Success && int.TryParse(match.Groups[1].Value, out int pct))
                        {
                            ProgressBarUpload.Value = pct;
                            TxtProgressPercent.Text = $"{pct}%";
                        }
                    }
                    else if (argsData.Data.StartsWith("SUCCESS:"))
                    {
                        GererEtatInterface(true);
                        ProgressBarUpload.Value = 100;
                        TxtProgressPercent.Text = "100%";
                        Close(true);
                    }
                    else if (argsData.Data.StartsWith("ERROR:"))
                    {
                        GererEtatInterface(true);
                        ProgressSection.IsVisible = false;
                        _ = new MessageDialog("Erreur Steam", argsData.Data.Replace("ERROR:", "")).ShowDialog(this);
                    }
                });
            };

            workerProcess.Start();
            workerProcess.BeginOutputReadLine();
        }

        private void GererEtatInterface(bool actif)
        {
            TxtTitle.IsEnabled = actif;
            TxtDescription.IsEnabled = actif;
            DropZonePak.IsEnabled = actif;
            TagsPanel.IsEnabled = actif;
            BtnPublishMod.IsEnabled = actif;
            BtnCloseWindowPublish.IsEnabled = actif;

            if (actif) BtnPublishMod.Content = estUneCreation ? "Publish Mod" : "Update Mod";
        }
    }
}
