using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.Diagnostics;

namespace UpGun_Mod_Tools_Launcher.Forms
{
    public partial class Form2 : Window
    {
        private uint _appId;
        private Steamworks.PublishedFileId_t _fileId;

        // Constructeur requis pour le designer XAML
        public Form2()
        {
            InitializeComponent();
        }

        public Form2(uint appId, string title, string description, string tags, Steamworks.PublishedFileId_t fileId, string previewUrl)
        {
            InitializeComponent();
            _appId = appId;
            _fileId = fileId;

            TxtTitle.Text = title;
            TxtDescription.Text = description;
            TxtTags.Text = tags;

            Title = (fileId.m_PublishedFileId == 0) ? "Publish New Mod" : "Update Mod";
            BtnSubmit.Content = (fileId.m_PublishedFileId == 0) ? "Upload" : "Update";
        }

        private async void BtnBrowseImage_Click(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select Preview Image",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Images") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg" } }
                }
            });

            if (files.Count > 0)
            {
                TxtImagePath.Text = files[0].Path.LocalPath;
            }
        }

        private async void BtnBrowseFolder_Click(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Mod Content Directory",
                AllowMultiple = false
            });

            if (folders.Count > 0)
            {
                TxtFolderPath.Text = folders[0].Path.LocalPath;
            }
        }

        private void BtnSubmit_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitle.Text) || string.IsNullOrWhiteSpace(TxtFolderPath.Text))
            {
                // Validation simple : Titre et dossier obligatoires
                return;
            }

            string? exePath = Environment.ProcessPath;
            if (string.IsNullOrEmpty(exePath)) return;

            // Encodage en Base64 pour correspondre aux attentes du worker Steam
            string b64Title = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(TxtTitle.Text));
            string b64Desc = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(TxtDescription.Text ?? ""));
            string b64Folder = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(TxtFolderPath.Text));
            string b64Image = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(TxtImagePath.Text ?? ""));
            string b64Tags = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(TxtTags.Text ?? ""));

            // Ordre strict attendu par ExecuteWorkshopPublish : 
            // [--worker-publish] [AppId] [FileId] [Title] [Desc] [PakPath] [IconPath] [Tags]
            string arguments = $"--worker-publish {_appId} {_fileId.m_PublishedFileId} \"{b64Title}\" \"{b64Desc}\" \"{b64Folder}\" \"{b64Image}\" \"{b64Tags}\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using Process process = Process.Start(startInfo)!;

            Console.WriteLine(process.StandardOutput.ReadToEnd());
            Console.WriteLine(process.StandardError.ReadToEnd());

            process.WaitForExit();

            Close(true); // Ferme la fenêtre avec succès
        }

        private void BtnCancel_Click(object? sender, RoutedEventArgs e)
        {
            Close(false);
        }
    }
}