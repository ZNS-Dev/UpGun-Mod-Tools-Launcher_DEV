using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UpGun_Mod_Tools_Launcher.Forms
{
    public partial class Form2 : Window
    {
        private readonly uint m_TargetAppId;
        private string _selectedIconPath = "";
        private PublishedFileId_t m_FileId;
        private readonly bool _isCreation = false;
        private Process? _workerProcess;
        private bool _windowClosed = false;

        public Form2()
        {
            InitializeComponent();

            // Build tag list
            string[] tagNames = new[]
            {
                "AK-Thodik", "Animation", "Armor", "Audio",
                "Character", "Face", "Gamemode", "Knife",
                "Map", "Model", "Settings", "UI",
                "Upgrade", "Weapon", "WIP"
            };
            foreach (var tag in tagNames)
                TagsListBox.Items.Add(new TagItem { Label = tag });

            // Set up drag & drop events
            AddDragDrop(BtnSelectPak, ".pak", "Please drop a file in .pak format!");
            AddDragDrop(BtnSelectIcon, ".png", "Please drop an image in .png format!");
            AddDragDrop(ImageIconPreview, ".png", "Please drop an image in .png format!");
            AddDragDrop(IconPreviewBorder, ".png", "Please drop an image in .png format!");
            AddDragDrop(TxtIconPath, ".png", "Please drop an image in .png format!");
            AddDragDrop(PathPak, ".pak", "Please drop a file in .pak format!");

            TxtProgressPercent.IsVisible = false;
            this.Closed += (s, e) => _windowClosed = true;
        }

        public Form2(uint targetAppId, string modTitle, string modDescription, string modTags, PublishedFileId_t fileId, string previewUrl = "")
            : this()
        {
            m_TargetAppId = targetAppId;
            m_FileId = fileId;

            TxtTitle.Text = modTitle;
            TxtDescription.Text = modDescription;

            _isCreation = (m_FileId == PublishedFileId_t.Invalid || m_FileId.m_PublishedFileId == 0);
            BtnPublishMod.Content = _isCreation ? "Publish Mod" : "Update Mod";

            // Restore tags from comma-separated string
            if (!string.IsNullOrEmpty(modTags))
            {
                string[] tagList = modTags.Split(',').Select(t => t.Trim()).ToArray();
                foreach (TagItem tagItem in TagsListBox.Items)
                {
                    if (tagList.Contains(tagItem.Label, StringComparer.OrdinalIgnoreCase))
                        tagItem.IsChecked = true;
                }
                RefreshTagsListBox();
            }

            if (!string.IsNullOrEmpty(previewUrl) && Uri.TryCreate(previewUrl, UriKind.Absolute, out var uri))
            {
                TxtIconPath.Text = "(Workshop Icon)";
                try
                {
                    var webClient = new System.Net.WebClient();
                    webClient.DownloadDataCompleted += (s, e) =>
                    {
                        if (e.Error == null && e.Result != null)
                        {
                            try
                            {
                                using var ms = new MemoryStream(e.Result);
                                var bitmap = new Bitmap(ms);
                                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                                {
                                    ImageIconPreview.Source = bitmap;
                                });
                            }
                            catch { }
                        }
                    };
                    webClient.DownloadDataAsync(uri);
                }
                catch { }
            }

            Title = _isCreation ? "Publish New Mod" : "Update Mod";
        }

        private void AddDragDrop(Control control, string extension, string errorMessage)
        {
            control.AddHandler(DragDrop.DragEnterEvent, (sender, e) =>
            {
                if (e.Data.Contains(DataFormats.Files))
                    e.DragEffects = DragDropEffects.Copy;
                else
                    e.DragEffects = DragDropEffects.None;
            });

            control.AddHandler(DragDrop.DropEvent, (sender, e) =>
            {
                if (e.Data.Contains(DataFormats.Files))
                {
                    var files = e.Data.GetFiles();
                    if (files != null)
                    {
                        var file = files.FirstOrDefault();
                        if (file != null)
                        {
                            string path = file.Path.LocalPath;
                            if (Path.GetExtension(path).Equals(extension, StringComparison.OrdinalIgnoreCase))
                            {
                                if (extension == ".pak")
                                    PathPak.Text = path;
                                else
                                    LoadIconFromFile(path);
                            }
                            else
                            {
                                _ = MessageBox.Show(this, errorMessage, "Unsupported File");
                            }
                        }
                    }
                }
            });
        }

        private void RefreshTagsListBox()
        {
            var items = TagsListBox.Items.ToList();
            TagsListBox.Items.Clear();
            foreach (var item in items)
                TagsListBox.Items.Add(item);
        }

        private async void BtnSelectPak_Click(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select .pak File",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Pak file") { Patterns = new[] { "*.pak" } }
                }
            });

            if (files.Count > 0)
                PathPak.Text = files[0].Path.LocalPath;
        }

        private async void BtnSelectIcon_Click(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select Icon (.png)",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("PNG Image") { Patterns = new[] { "*.png" } }
                }
            });

            if (files.Count > 0)
                LoadIconFromFile(files[0].Path.LocalPath);
        }

        private void LoadIconFromFile(string filePath)
        {
            _selectedIconPath = filePath;
            TxtIconPath.Text = filePath;

            try
            {
                if (File.Exists(filePath))
                {
                    using var stream = File.OpenRead(filePath);
                    var bitmap = new Bitmap(stream);
                    ImageIconPreview.Source = bitmap;

                    if (bitmap.PixelSize.Width != 512 || bitmap.PixelSize.Height != 512)
                    {
                        _ = MessageBox.Show(this,
                            $"Selected image is {bitmap.PixelSize.Width}x{bitmap.PixelSize.Height} pixels.\n\nSteam strongly recommends an exact size of 512x512 pixels for mod icons.",
                            "Warning - Non-recommended Icon Dimensions");
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(this, "Unable to load icon preview: " + ex.Message, "Error");
                ImageIconPreview.Source = null;
            }
        }

        private async void BtnPublishMod_Click(object? sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(PathPak.Text) || !File.Exists(PathPak.Text) ||
                !Path.GetExtension(PathPak.Text).Equals(".pak", StringComparison.OrdinalIgnoreCase))
            {
                await MessageBox.Show(this, "Invalid .pak file!", "Error");
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtTitle.Text))
            {
                await MessageBox.Show(this, "Please enter a title.", "Missing Title");
                return;
            }

            // Collect checked tags
            List<string> checkedTags = new List<string>();
            foreach (TagItem tagItem in TagsListBox.Items)
            {
                if (tagItem.IsChecked)
                    checkedTags.Add(tagItem.Label);
            }

            ManageUIState(false);

            string titleB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(TxtTitle.Text.Trim()));
            string descB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(TxtDescription.Text ?? ""));
            string pakB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(PathPak.Text));
            string iconB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(_selectedIconPath ?? ""));
            string tagsB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(string.Join(",", checkedTags)));

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Environment.ProcessPath,
                Arguments = $"--worker-publish {m_TargetAppId} {m_FileId.m_PublishedFileId} \"{titleB64}\" \"{descB64}\" \"{pakB64}\" \"{iconB64}\" \"{tagsB64}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            _workerProcess = new Process { StartInfo = startInfo };

            _workerProcess.OutputDataReceived += (s, argsData) =>
            {
                if (string.IsNullOrEmpty(argsData.Data) || _windowClosed) return;

                try
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
                    {
                        if (_windowClosed) return;

                        if (argsData.Data.StartsWith("PROGRESS:"))
                        {
                            string progressMsg = argsData.Data.Replace("PROGRESS:", "");
                            BtnPublishMod.Content = progressMsg;

                            int openParen = progressMsg.LastIndexOf('(');
                            int closeParen = progressMsg.LastIndexOf('%');

                            if (openParen != -1 && closeParen != -1 && closeParen > openParen)
                            {
                                string percentStr = progressMsg.Substring(openParen + 1, closeParen - openParen - 1);
                                if (int.TryParse(percentStr, out int percentValue))
                                {
                                    ProgressBar.IsIndeterminate = false;
                                    ProgressBar.Value = Math.Min(100, Math.Max(0, percentValue));
                                    TxtProgressPercent.Text = $"{percentValue}%";
                                }
                            }
                            else
                            {
                                ProgressBar.IsIndeterminate = true;
                                TxtProgressPercent.Text = "...";
                            }
                        }
                        else if (argsData.Data.StartsWith("SUCCESS:"))
                        {
                            ManageUIState(true);
                            string generatedFileId = argsData.Data.Replace("SUCCESS:", "");

                            await MessageBox.Show(this, "Publish successful! Opening Workshop page...", "Success");

                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "steam://url/CommunityFilePage/" + generatedFileId,
                                UseShellExecute = true
                            });

                            Close();
                        }
                        else if (argsData.Data.StartsWith("ERROR:"))
                        {
                            ManageUIState(true);
                            await MessageBox.Show(this, argsData.Data.Replace("ERROR:", ""), "Steam Error");
                        }
                    });
                }
                catch (Exception) { }
            };

            _workerProcess.Start();
            _workerProcess.BeginOutputReadLine();
        }

        private void BtnCancel_Click(object? sender, RoutedEventArgs e)
        {
            if (_workerProcess != null && !_workerProcess.HasExited)
            {
                try { _workerProcess.Kill(); } catch { }
            }
            Close();
        }

        private void ManageUIState(bool active)
        {
            BtnSelectPak.IsEnabled = active;
            TxtTitle.IsEnabled = active;
            TxtDescription.IsEnabled = active;
            BtnSelectIcon.IsEnabled = active;
            TagsListBox.IsEnabled = active;
            BtnPublishMod.IsEnabled = active;
            BtnCancel.IsEnabled = active;

            if (active)
            {
                BtnPublishMod.Content = _isCreation ? "Publish Mod" : "Update Mod";
                ProgressBar.Value = 0;
                ProgressBar.IsIndeterminate = false;
                TxtProgressPercent.Text = "0%";
                TxtProgressPercent.IsVisible = false;
            }
            else
            {
                TxtProgressPercent.IsVisible = true;
            }
        }
    }

    public class TagItem : INotifyPropertyChanged
    {
        private bool _isChecked;
        public string Label { get; set; } = "";

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}