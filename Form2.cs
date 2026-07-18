using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Steamworks;

namespace UpGun_Mods_Tool_Launcher
{
    public partial class Form2 : Form
    {
        private const uint APP_ID_CIBLE = 311210;

        private string SelectFileIcon = "";
        private string dossierTemporaireUpload = "";
        private PublishedFileId_t m_FileId;
        private CallResult<SubmitItemUpdateResult_t> m_SubmitItemUpdate;
        private CallResult<CreateItemResult_t> m_CreateItem;

        private UGCUpdateHandle_t m_CurrentUpdateHandle;
        private Timer progressTimer;
        private bool estUneCreation = false;

        public Form2()
        {
            InitializeComponent();

            // Sécurité sur la fermeture
            this.FormClosing -= Form2_FormClosing;
            this.FormClosing += Form2_FormClosing;

            // 🛠️ REMÈDE ANTI-BUG DU DESIGNER : On injecte les Tags directement par le code.
            if (checkedListBox1.Items.Count == 0)
            {
                checkedListBox1.Items.AddRange(new object[] {
                    "Map",
                    "Mod",
                    "Skin",
                    "Gamemode",
                    "Weapon",
                    "Other"
                });
            }

            // 🔒 NETTOYAGE DES DOUBLONS : On fait -= puis += pour détruire les doubles lancements
            try { this.BtnSelectPak.Click -= this.BtnSelectPak_Click; this.BtnSelectPak.Click += this.BtnSelectPak_Click; } catch { }
            try { this.button1.Click -= this.button1_Click; this.button1.Click += this.button1_Click; } catch { }
            try { this.button2.Click -= this.button2_Click; this.button2.Click += this.button2_Click; } catch { }
            try { this.button3.Click -= this.button3_Click; this.button3.Click += this.button3_Click; } catch { }
        }

        public Form2(string titreMod, string descriptionMod, string tagsMod, PublishedFileId_t fileId) : this()
        {
            m_FileId = fileId;
            m_SubmitItemUpdate = CallResult<SubmitItemUpdateResult_t>.Create(OnItemUpdateCompleted);
            m_CreateItem = CallResult<CreateItemResult_t>.Create(OnItemCreateCompleted);

            textBox2.Text = titreMod;
            textBox3.Text = descriptionMod;

            if (m_FileId == PublishedFileId_t.Invalid)
            {
                estUneCreation = true;
                button3.Text = "Publish New Map";
            }
            else
            {
                estUneCreation = false;
                button3.Text = "Update Map";
            }

            // Cocher automatiquement les tags récupérés depuis Steam
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
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Fichier Pak (*.pak)|*.pak";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    PathPak.Text = openFileDialog.FileName;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PathPak.Text) || !File.Exists(PathPak.Text) || Path.GetExtension(PathPak.Text).ToLower() != ".pak")
            {
                MessageBox.Show("Erreur : Vous devez obligatoirement sélectionner un fichier de Mod valide (.pak) avant de publier !", "Fichier .pak manquant", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(textBox2.Text.Trim()))
            {
                MessageBox.Show("Veuillez donner un titre à votre map.", "Titre manquant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            progressBar1.Value = 0;
            button3.Enabled = false;

            if (estUneCreation)
            {
                AppId_t appId = new AppId_t(APP_ID_CIBLE);
                SteamAPICall_t apiCall = SteamUGC.CreateItem(appId, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
                m_CreateItem.Set(apiCall);
                MessageBox.Show("Enregistrement de la nouvelle map sur Steam... Veuillez patienter.", "Création", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                button3.Enabled = true;
                MessageBox.Show("Impossible de créer l'article sur le Workshop Steam. Code : " + callback.m_eResult, "Erreur Steam", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            m_FileId = callback.m_nPublishedFileId;
            DemarrerSoumissionWorkshop();
        }

        private void DemarrerSoumissionWorkshop()
        {
            try
            {
                AppId_t appId = new AppId_t(APP_ID_CIBLE);
                m_CurrentUpdateHandle = SteamUGC.StartItemUpdate(appId, m_FileId);

                SteamUGC.SetItemTitle(m_CurrentUpdateHandle, textBox2.Text);
                SteamUGC.SetItemDescription(m_CurrentUpdateHandle, textBox3.Text);

                dossierTemporaireUpload = Path.Combine(Path.GetTempPath(), "UpGun_Upload_" + Guid.NewGuid().ToString("N"));
                if (Directory.Exists(dossierTemporaireUpload))
                {
                    Directory.Delete(dossierTemporaireUpload, true);
                }
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

                SteamAPICall_t apiCall = SteamUGC.SubmitItemUpdate(m_CurrentUpdateHandle, "Mise à jour via BO3 Mods Tool");
                m_SubmitItemUpdate.Set(apiCall);

                progressTimer = new Timer();
                progressTimer.Interval = 200;
                progressTimer.Tick += ProgressTimer_Tick;
                progressTimer.Start();
            }
            catch (Exception ex)
            {
                NettoyerDossierTemporaire();
                button3.Enabled = true;
                MessageBox.Show("Erreur lors de la préparation de l'envoi : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            ulong bytesProcessed = 0;
            ulong bytesTotal = 0;
            SteamUGC.GetItemUpdateProgress(m_CurrentUpdateHandle, out bytesProcessed, out bytesTotal);

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
            progressTimer?.Stop();
            button3.Enabled = true;

            NettoyerDossierTemporaire();

            if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
            {
                progressBar1.Value = 0;
                MessageBox.Show("Échec de l'opération sur Steam. Code : " + callback.m_eResult, "Erreur Steam", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                progressBar1.Value = 100;
                string messageSucces = estUneCreation ? "Nouvelle map publiée avec succès sur le Workshop !" : "Map mise à jour avec succès !";
                MessageBox.Show(messageSucces, "Succès !", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            NettoyerDossierTemporaire();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Sélectionner l'icône du mod";
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Fichiers Image (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SelectFileIcon = openFileDialog.FileName;
                    textBox5.Text = SelectFileIcon;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) => this.Close();
    }
}