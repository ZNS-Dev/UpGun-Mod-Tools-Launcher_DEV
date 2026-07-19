using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace UpGun_Mods_Tool_Launcher
{
    public partial class Form2 : Form
    {
        private readonly uint m_AppIdCible;
        private string SelectFileIcon = "";
        private string dossierTemporaireUpload = "";
        private PublishedFileId_t m_FileId;
        private readonly CallResult<SubmitItemUpdateResult_t> m_SubmitItemUpdate;
        private readonly CallResult<CreateItemResult_t> m_CreateItem;
        private UGCUpdateHandle_t m_CurrentUpdateHandle;
        private readonly bool estUneCreation = false;

        public Form2()
        {
            InitializeComponent();
            this.FormClosing -= Form2_FormClosing;
            this.FormClosing += Form2_FormClosing;
            this.BtnSelectPak.Click -= this.BtnSelectPak_Click; this.BtnSelectPak.Click += this.BtnSelectPak_Click;
            this.BtnSelectIcon.Click -= this.BtnSelectIcon_Click; this.BtnSelectIcon.Click += this.BtnSelectIcon_Click;
            this.BtnCloseWindowPublish.Click -= this.BtnCloseWindowPublish_Click; this.BtnCloseWindowPublish.Click += this.BtnCloseWindowPublish_Click;
            this.BtnPublishMod.Click -= this.BtnPublishMod_Click; this.BtnPublishMod.Click += this.BtnPublishMod_Click;
        }

        public Form2(uint appIdCible, string titreMod, string descriptionMod, string tagsMod, PublishedFileId_t fileId) : this()
        {
            m_AppIdCible = appIdCible;
            m_FileId = fileId;
            m_SubmitItemUpdate = CallResult<SubmitItemUpdateResult_t>.Create(OnItemUpdateCompleted);
            m_CreateItem = CallResult<CreateItemResult_t>.Create(OnItemCreateCompleted);

            textBox2.Text = titreMod;
            textBox3.Text = descriptionMod;

            if (m_FileId == PublishedFileId_t.Invalid)
            {
                estUneCreation = true;
                BtnPublishMod.Text = "Publish New Map";
            }
            else
            {
                estUneCreation = false;
                BtnPublishMod.Text = "Update Map";
            }

            if (!string.IsNullOrEmpty(tagsMod))
            {
                string[] listeTagsSteam = tagsMod.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < listeTagsSteam.Length; i++)
                {
                    listeTagsSteam[i] = listeTagsSteam[i].Trim();
                }

                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    string nomDuTagDansLaListe = checkedListBox1.Items[i].ToString().Trim();
                    if (listeTagsSteam.Contains(nomDuTagDansLaListe, StringComparer.OrdinalIgnoreCase))
                    {
                        checkedListBox1.SetItemChecked(i, true);
                    }
                }
            }

            progressBar1.Value = 0;
            progressBar1.Maximum = 100;
        }

        private void BtnSelectPak_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Sélectionner le fichier du mod (.pak)";
                openFileDialog.Filter = "Fichier Pak (*.pak)|*.pak";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    PathPak.Text = openFileDialog.FileName;
                }
            }
        }

        private void BtnPublishMod_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PathPak.Text) || !File.Exists(PathPak.Text) || Path.GetExtension(PathPak.Text).ToLower() != ".pak")
            {
                MessageBox.Show("Erreur : Fichier .pak invalide !", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(textBox2.Text.Trim()))
            {
                MessageBox.Show("Veuillez donner un titre.", "Titre manquant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            progressBar1.Value = 0;
            BtnPublishMod.Enabled = false;

            if (estUneCreation)
            {
                AppId_t appId = new AppId_t(m_AppIdCible);
                SteamAPICall_t apiCall = SteamUGC.CreateItem(appId, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
                m_CreateItem.Set(apiCall);
            }
            else
            {
                DemarrerSoumissionWorkshop();
            }
        }

        private void OnItemCreateCompleted(CreateItemResult_t callback, bool bIOFailure)
        {
            if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
            {
                BtnPublishMod.Enabled = true;
                MessageBox.Show("Échec création Workshop. Code : " + callback.m_eResult, "Erreur Steam", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            m_FileId = callback.m_nPublishedFileId;
            DemarrerSoumissionWorkshop();
        }

        private void DemarrerSoumissionWorkshop()
        {
            try
            {
                AppId_t appId = new AppId_t(m_AppIdCible);
                m_CurrentUpdateHandle = SteamUGC.StartItemUpdate(appId, m_FileId);

                SteamUGC.SetItemTitle(m_CurrentUpdateHandle, textBox2.Text);
                SteamUGC.SetItemDescription(m_CurrentUpdateHandle, textBox3.Text);

                dossierTemporaireUpload = Path.Combine(Path.GetTempPath(), "UpGun_Upload_" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(dossierTemporaireUpload);

                string nomDuPak = Path.GetFileName(PathPak.Text);
                string cheminTemporairePak = Path.Combine(dossierTemporaireUpload, nomDuPak);
                File.Copy(PathPak.Text, cheminTemporairePak, true);

                SteamUGC.SetItemContent(m_CurrentUpdateHandle, dossierTemporaireUpload);

                if (!string.IsNullOrEmpty(SelectFileIcon) && File.Exists(SelectFileIcon))
                {
                    FileInfo imgInfo = new FileInfo(SelectFileIcon);
                    if (imgInfo.Length <= 1024 * 1024)
                    {
                        SteamUGC.SetItemPreview(m_CurrentUpdateHandle, SelectFileIcon);
                    }
                }

                List<string> tagsCoches = new List<string>();
                foreach (var item in checkedListBox1.CheckedItems)
                {
                    tagsCoches.Add(item.ToString());
                }
                SteamUGC.SetItemTags(m_CurrentUpdateHandle, tagsCoches);

                SteamAPICall_t apiCall = SteamUGC.SubmitItemUpdate(m_CurrentUpdateHandle, "Mise à jour");
                m_SubmitItemUpdate.Set(apiCall);
            }
            catch (Exception ex)
            {
                NettoyerDossierTemporaire();
                BtnPublishMod.Enabled = true;
                MessageBox.Show("Erreur préparation : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            SteamUGC.GetItemUpdateProgress(m_CurrentUpdateHandle, out ulong bytesProcessed, out ulong bytesTotal);

            if (bytesTotal > 0)
            {
                int pourcentage = (int)((bytesProcessed * 100) / bytesTotal);
                if (pourcentage > 100) pourcentage = 100;
                if (pourcentage < 0) pourcentage = 0;
                progressBar1.Value = pourcentage;
            }
        }

        private void OnItemUpdateCompleted(SubmitItemUpdateResult_t callback, bool bIOFailure)
        {
            BtnPublishMod.Enabled = true;
            NettoyerDossierTemporaire();

            if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
            {
                MessageBox.Show("Échec sur Steam. Code : " + callback.m_eResult, "Erreur Steam", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                progressBar1.Value = 100;
                DialogResult reponse = MessageBox.Show("Opération réussie ! Ouvrir la page ?", "Succès", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (reponse == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("steam://url/CommunityFilePage/" + m_FileId.m_PublishedFileId);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void NettoyerDossierTemporaire()
        {
            try
            {
                if (!string.IsNullOrEmpty(dossierTemporaireUpload) && Directory.Exists(dossierTemporaireUpload))
                {
                    Directory.Delete(dossierTemporaireUpload, true);
                }
            }
            catch { }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e) => NettoyerDossierTemporaire();

        private void BtnSelectIcon_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Sélectionner l'icône du mod";
                openFileDialog.Filter = "Fichiers Image (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SelectFileIcon = openFileDialog.FileName;
                    textBox5.Text = SelectFileIcon;
                }
            }
        }

        private void BtnCloseWindowPublish_Click(object sender, EventArgs e) => this.Close();
    }
}