using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices; // 🌟 Indispensable pour l'injection native
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Steamworks;

namespace UpGun_Mods_Tool_Launcher
{
    public partial class Form1 : Form
    {
        // 🆔 L'ID unique de Steam gravé dans le code
        private const uint APP_ID_CIBLE = 480;

        // 🚀 L'arme secrète : On importe la fonction native de Windows pour forcer l'ID en mémoire
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetEnvironmentVariable(string lpName, string lpValue);

        private CallResult<SteamUGCQueryCompleted_t> m_SteamUGCQueryCompleted;
        private Timer steamTimer;

        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;

            this.Load += new System.EventHandler(this.Form1_Load);
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);

            this.BtnUpload.Click -= new System.EventHandler(this.BtnUpload_Click_1);
            this.BtnUpload.Click += new System.EventHandler(this.BtnUpload_Click_1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ChargerWorkshopSteam();
            MettreAJourTexteBouton();
        }

        private void ChargerWorkshopSteam()
        {
            try
            {
                // 🪄 INJECTION DIRECTE : On force l'ID dans le kernel Windows pour le processus actuel.
                // La DLL de Steam (steam_api.dll) est obligée de le lire ici, rendant le fichier TXT inutile !
                SetEnvironmentVariable("SteamAppId", APP_ID_CIBLE.ToString());

                if (SteamAPI.Init())
                {
                    if (steamTimer == null)
                    {
                        steamTimer = new Timer();
                        steamTimer.Interval = 100;
                        steamTimer.Tick += (s, e) => SteamAPI.RunCallbacks();
                        steamTimer.Start();
                    }

                    m_SteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create(OnWorkshopQueryCompleted);

                    AppId_t jeuCible = new AppId_t(APP_ID_CIBLE);

                    // Requête utilisateur globale pour récupérer TOUS tes items associés à cet ID
                    UGCQueryHandle_t handle = SteamUGC.CreateQueryUserUGCRequest(
                        SteamUser.GetSteamID().GetAccountID(),
                        EUserUGCList.k_EUserUGCList_Published,
                        EUGCMatchingUGCType.k_EUGCMatchingUGCType_All, // 'All' pour ne rater aucun type d'item
                        EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc,
                        AppId_t.Invalid,
                        jeuCible, // Cible l'ID du jeu pour filtrer tes items
                        1
                    );

                    SteamAPICall_t apiCall = SteamUGC.SendQueryUGCRequest(handle);
                    m_SteamUGCQueryCompleted.Set(apiCall);
                }
                else
                {
                    MessageBox.Show($"Impossible de se connecter à Steam.\nVérifiez que Steam est lancé en arrière-plan.", "Erreur Steam", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur SDK Steamworks : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnWorkshopQueryCompleted(SteamUGCQueryCompleted_t callback, bool bIOFailure)
        {
            checkedListBox1.Items.Clear();

            if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
            {
                MessageBox.Show("Erreur Workshop Steam (Code : " + callback.m_eResult + ")", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (callback.m_unNumResultsReturned > 0)
            {
                for (uint i = 0; i < callback.m_unNumResultsReturned; i++)
                {
                    SteamUGCDetails_t details;
                    if (SteamUGC.GetQueryUGCResult(callback.m_handle, i, out details))
                    {
                        WorkshopItem item = new WorkshopItem
                        {
                            Title = details.m_rgchTitle,
                            Description = details.m_rgchDescription,
                            Tags = details.m_rgchTags,
                            FileId = details.m_nPublishedFileId
                        };
                        checkedListBox1.Items.Add(item);
                    }
                }
            }

            SteamUGC.ReleaseQueryUGCRequest(callback.m_handle);
            MettreAJourTexteBouton();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MettreAJourTexteBouton();
        }

        private void MettreAJourTexteBouton()
        {
            BtnUpload.Text = (checkedListBox1.SelectedItem != null) ? "Update" : "Upload";
        }

        private void BtnUpload_Click_1(object sender, EventArgs e)
        {
            Form2 form2;

            if (checkedListBox1.SelectedItem is WorkshopItem modSelectionne)
            {
                form2 = new Form2(APP_ID_CIBLE, modSelectionne.Title, modSelectionne.Description, modSelectionne.Tags, modSelectionne.FileId);
            }
            else
            {
                form2 = new Form2(APP_ID_CIBLE, "", "", "", PublishedFileId_t.Invalid);
            }

            if (form2.ShowDialog() == DialogResult.OK)
            {
                ChargerWorkshopSteam();
            }
        }

        private void BtnStartUpGun_Click(object sender, EventArgs e)
        {
            Process.Start("steam://rungameid/" + APP_ID_CIBLE);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (steamTimer != null)
            {
                steamTimer.Stop();
                steamTimer.Dispose();
            }
            SteamAPI.Shutdown();
            // Plus aucun code de nettoyage de fichier ici, le dossier reste impeccable.
        }
    }

    public class WorkshopItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public PublishedFileId_t FileId { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Title) ? "Mod sans titre (ID: " + FileId.m_PublishedFileId + ")" : Title;
        }
    }
}