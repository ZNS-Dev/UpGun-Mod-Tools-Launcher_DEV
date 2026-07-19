using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace UpGun_Mod_Tools_Launcher
{
    public partial class Form2 : Form
    {
        private readonly uint m_AppIdCible;
        private string SelectFileIcon = "";
        private PublishedFileId_t m_FileId;
        private readonly CallResult<SubmitItemUpdateResult_t> m_SubmitItemUpdate;
        private readonly CallResult<CreateItemResult_t> m_CreateItem;
        private UGCUpdateHandle_t m_CurrentUpdateHandle;
        private readonly bool estUneCreation = false;
        private readonly string dossierEnvoiUnique = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DONT_DELETE");

        public Form2()
        {
            InitializeComponent();
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
                BtnPublishMod.Text = "Publish Mod";
            }
            else
            {
                estUneCreation = false;
                BtnPublishMod.Text = "Update Mod";
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
        }

        private void GererEtatInterface(bool actif)
        {
            BtnSelectPak.Enabled = actif;
            textBox2.Enabled = actif;
            textBox3.Enabled = actif;
            BtnSelectIcon.Enabled = actif;
            checkedListBox1.Enabled = actif;
            BtnPublishMod.Enabled = actif;
            BtnCloseWindowPublish.Enabled = actif;
        }

        private void NettoyerDossierPassage()
        {
            try
            {
                if (Directory.Exists(dossierEnvoiUnique))
                {
                    Directory.Delete(dossierEnvoiUnique, true);
                }
            }
            catch
            {
                // Évite de faire crasher l'application si le dossier est verrouillé une demi-seconde par Windows
            }
        }

        private void BtnSelectPak_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select pak file (.pak)";
                openFileDialog.Filter = "Pak file (*.pak)|*.pak";
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
                MessageBox.Show("Invalid pak file!", "Error file pak", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(textBox2.Text.Trim()))
            {
                MessageBox.Show("Please make title for you mod.", "Missing title", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(SelectFileIcon))
            {
                if (!File.Exists(SelectFileIcon))
                {
                    MessageBox.Show("The selected icon was not found.", "Error icon", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                FileInfo imgInfo = new FileInfo(SelectFileIcon);
                if (imgInfo.Length > 1024 * 1024)
                {
                    MessageBox.Show("The icon must not exceed 1MB.", "Error icon", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            GererEtatInterface(false);

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
                GererEtatInterface(true);
                MessageBox.Show("Failed to create mod. Code: " + callback.m_eResult, "Error Steam", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                // ÉTAPE SÉCURITÉ : On nettoie et on recrée à neuf le mini dossier d'isolement
                NettoyerDossierPassage();
                Directory.CreateDirectory(dossierEnvoiUnique);

                // On copie UNIQUEMENT le fichier .pak que tu as sélectionné
                string nomDuPak = Path.GetFileName(PathPak.Text);
                string cheminSecurisePak = Path.Combine(dossierEnvoiUnique, nomDuPak);
                File.Copy(PathPak.Text, cheminSecurisePak, true);

                // On donne ce dossier propre à Steam (il ne contient QUE ton .pak, plus aucun risque de dossier à 1.2 Go)
                SteamUGC.SetItemContent(m_CurrentUpdateHandle, dossierEnvoiUnique);

                if (!string.IsNullOrEmpty(SelectFileIcon) && File.Exists(SelectFileIcon))
                {
                    SteamUGC.SetItemPreview(m_CurrentUpdateHandle, SelectFileIcon);
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
                GererEtatInterface(true);
                NettoyerDossierPassage(); // En cas de plantage, on efface tout de suite
                MessageBox.Show("Erreur préparation : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnItemUpdateCompleted(SubmitItemUpdateResult_t callback, bool bIOFailure)
        {
            GererEtatInterface(true);

            // NETTOYAGE TOTAL : Steam a fini de lire les fichiers, on détruit instantanément le dossier d'envoi
            NettoyerDossierPassage();

            if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
            {
                MessageBox.Show("Échec sur Steam. Code : " + callback.m_eResult, "Erreur Steam", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult reponse = MessageBox.Show("Opération réussie ! Ouvrir la page ?", "Succès", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (reponse == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("steam://url/CommunityFilePage/" + m_FileId.m_PublishedFileId);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnSelectIcon_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select icon";
                openFileDialog.Filter = "Icon file (*.png)|*.png";
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