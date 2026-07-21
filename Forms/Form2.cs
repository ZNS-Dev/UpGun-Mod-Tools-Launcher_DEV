using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace UpGun_Mod_Tools_Launcher
{
    public partial class Form2 : Form
    {
        private readonly uint m_TargetAppId;
        private string SelectFileIcon = "";
        private PublishedFileId_t m_FileId;
        private readonly bool isCreation = false;
        private Process workerProcess;

        public Form2()
        {
            InitializeComponent();

            TxtBoxPourcent.Visible = false;

            this.BtnSelectPak.Click -= SelectPakFile_Click;
            this.BtnSelectPak.Click += SelectPakFile_Click;

            this.BtnSelectIcon.Click -= SelectIcon_Click;
            this.BtnSelectIcon.Click += SelectIcon_Click;

            this.BtnCloseWindowPublish.Click -= BtnCloseWindowPublish_Click;
            this.BtnCloseWindowPublish.Click += BtnCloseWindowPublish_Click;

            this.BtnPublishMod.Click -= BtnPublishMod_Click;
            this.BtnPublishMod.Click += BtnPublishMod_Click;

            this.BtnSelectPak.AllowDrop = true;
            this.BtnSelectPak.DragEnter += PathPak_DragEnter;
            this.BtnSelectPak.DragDrop += PathPak_DragDrop;

            this.PathPak.AllowDrop = true;
            this.PathPak.DragEnter += PathPak_DragEnter;
            this.PathPak.DragDrop += PathPak_DragDrop;

            this.BtnSelectIcon.AllowDrop = true;
            this.BtnSelectIcon.DragEnter += Icon_DragEnter;
            this.BtnSelectIcon.DragDrop += Icon_DragDrop;

            this.textBox5.AllowDrop = true;
            this.textBox5.DragEnter += Icon_DragEnter;
            this.textBox5.DragDrop += Icon_DragDrop;

            this.ImageIconPreview.AllowDrop = true;
            this.ImageIconPreview.DragEnter += Icon_DragEnter;
            this.ImageIconPreview.DragDrop += Icon_DragDrop;
        }

        private void BtnCloseWindowPublish_Click(object sender, EventArgs e) => this.Close();

        public Form2(uint targetAppId, string modTitle, string modDescription, string modTags, PublishedFileId_t fileId, string previewUrl = "") : this()
        {
            m_TargetAppId = targetAppId;
            m_FileId = fileId;

            textBox2.Text = modTitle;
            textBox3.Text = modDescription;

            isCreation = (m_FileId == PublishedFileId_t.Invalid);
            BtnPublishMod.Text = isCreation ? "Publish Mod" : "Update Mod";

            if (!string.IsNullOrEmpty(modTags))
            {
                string[] tagList = modTags.Split(',').Select(t => t.Trim()).ToArray();
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (tagList.Contains(checkedListBox1.Items[i].ToString().Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        checkedListBox1.SetItemChecked(i, true);
                    }
                }
            }

            if (!string.IsNullOrEmpty(previewUrl))
            {
                textBox5.Text = "(Workshop Icon)";
                try
                {
                    ImageIconPreview.SizeMode = PictureBoxSizeMode.Zoom;
                    ImageIconPreview.LoadAsync(previewUrl);
                }
                catch { }
            }
        }

        private void BtnPublishMod_Click(object sender, EventArgs e)
        {
            BtnPublishMod.Enabled = false;
            BtnCloseWindowPublish.Enabled = false;
            BtnSelectIcon.Enabled = false;

            if (string.IsNullOrEmpty(PathPak.Text) || !File.Exists(PathPak.Text) || Path.GetExtension(PathPak.Text).ToLower() != ".pak")
            {
                MessageBox.Show("Invalid .pak file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                BtnPublishMod.Enabled = true;
                BtnCloseWindowPublish.Enabled = true;
                BtnSelectIcon.Enabled = true;
                return;
            }

            if (string.IsNullOrEmpty(textBox2.Text.Trim()))
            {
                MessageBox.Show("Please enter a title.", "Missing Title", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                BtnPublishMod.Enabled = true;
                BtnCloseWindowPublish.Enabled = true;
                BtnSelectIcon.Enabled = true;
                return;
            }

            List<string> checkedTags = new List<string>();
            foreach (var item in checkedListBox1.CheckedItems)
            {
                checkedTags.Add(item.ToString());
            }

            ManageUIState(false);

            string titleB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(textBox2.Text.Trim()));
            string descB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(textBox3.Text.Trim()));
            string pakB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(PathPak.Text));
            string iconB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(SelectFileIcon ?? ""));
            string tagsB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(string.Join(",", checkedTags)));

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Application.ExecutablePath,
                Arguments = $"--worker-publish {m_TargetAppId} {m_FileId.m_PublishedFileId} \"{titleB64}\" \"{descB64}\" \"{pakB64}\" \"{iconB64}\" \"{tagsB64}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            workerProcess = new Process { StartInfo = startInfo };

            workerProcess.OutputDataReceived += (s, argsData) =>
            {
                if (string.IsNullOrEmpty(argsData.Data) || this.IsDisposed || !this.IsHandleCreated) return;

                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (this.IsDisposed) return;

                        if (argsData.Data.StartsWith("PROGRESS:"))
                        {
                            string progressMsg = argsData.Data.Replace("PROGRESS:", "");
                            BtnPublishMod.Text = progressMsg;

                            int openParen = progressMsg.LastIndexOf('(');
                            int closeParen = progressMsg.LastIndexOf('%');

                            if (openParen != -1 && closeParen != -1 && closeParen > openParen)
                            {
                                string percentStr = progressMsg.Substring(openParen + 1, closeParen - openParen - 1);
                                if (int.TryParse(percentStr, out int percentValue))
                                {
                                    progressBar1.Style = ProgressBarStyle.Blocks;
                                    progressBar1.Value = Math.Min(100, Math.Max(0, percentValue));
                                    TxtBoxPourcent.Text = $"{percentValue}%";
                                }
                            }
                            else
                            {
                                progressBar1.Style = ProgressBarStyle.Marquee;
                                TxtBoxPourcent.Text = "...";
                            }
                        }
                        else if (argsData.Data.StartsWith("SUCCESS:"))
                        {
                            ManageUIState(true);
                            string generatedFileId = argsData.Data.Replace("SUCCESS:", "");

                            if (MessageBox.Show("Publish successful! Open the Workshop page?", "Success", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                Process.Start("steam://url/CommunityFilePage/" + generatedFileId);
                            }

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else if (argsData.Data.StartsWith("ERROR:"))
                        {
                            ManageUIState(true);
                            MessageBox.Show(argsData.Data.Replace("ERROR:", ""), "Steam Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
                }
                catch (ObjectDisposedException) { }
            };

            workerProcess.Start();
            workerProcess.BeginOutputReadLine();
        }

        private void ManageUIState(bool active)
        {
            BtnSelectPak.Enabled = active;
            textBox2.Enabled = active;
            textBox3.Enabled = active;
            BtnSelectIcon.Enabled = active;
            checkedListBox1.Enabled = active;
            BtnPublishMod.Enabled = active;
            BtnCloseWindowPublish.Enabled = active;

            if (active)
            {
                BtnPublishMod.Text = isCreation ? "Publish Mod" : "Update Mod";
                progressBar1.Value = 0;
                progressBar1.Style = ProgressBarStyle.Blocks;
                TxtBoxPourcent.Text = "0%";
                TxtBoxPourcent.Visible = false;
            }
            else
            {
                TxtBoxPourcent.Visible = true;
            }
        }

        private void SelectPakFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Pak file (*.pak)|*.pak" })
            {
                if (ofd.ShowDialog() == DialogResult.OK) PathPak.Text = ofd.FileName;
            }
        }

        private void PathPak_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void PathPak_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                string droppedFile = files[0];
                if (Path.GetExtension(droppedFile).Equals(".pak", StringComparison.OrdinalIgnoreCase))
                {
                    PathPak.Text = droppedFile;
                }
                else
                {
                    MessageBox.Show("Please drop a file in .pak format!", "Unsupported File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void SelectIcon_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Icon file (*.png)|*.png" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    LoadIconFromFile(ofd.FileName);
                }
            }
        }

        private void Icon_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Icon_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                string droppedFile = files[0];
                if (Path.GetExtension(droppedFile).Equals(".png", StringComparison.OrdinalIgnoreCase))
                {
                    LoadIconFromFile(droppedFile);
                }
                else
                {
                    MessageBox.Show("Please drop an image in .png format!", "Unsupported Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void LoadIconFromFile(string filePath)
        {
            SelectFileIcon = filePath;
            textBox5.Text = SelectFileIcon;

            try
            {
                if (ImageIconPreview.Image != null)
                {
                    ImageIconPreview.Image.Dispose();
                    ImageIconPreview.Image = null;
                }

                if (File.Exists(SelectFileIcon))
                {
                    using (FileStream fs = new FileStream(SelectFileIcon, FileMode.Open, FileAccess.Read))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            fs.CopyTo(ms);
                            Image img = Image.FromStream(ms);

                            if (img.Width != 512 || img.Height != 512)
                            {
                                MessageBox.Show(
                                    $"Selected image is {img.Width}x{img.Height} pixels.\n\nSteam strongly recommends an exact size of 512x512 pixels for mod icons.",
                                    "Warning - Non-recommended Icon Dimensions",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                            }

                            ImageIconPreview.Image = img;
                        }
                    }

                    ImageIconPreview.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load icon preview: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ImageIconPreview.Image = null;
            }
        }
    }
}