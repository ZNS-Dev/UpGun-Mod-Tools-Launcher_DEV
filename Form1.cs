using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UpGun_Mod_Tools_Launcher
{
    public partial class Form1 : Form
    {
        private const uint APP_ID_CIBLE = 1575870;
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetEnvironmentVariable(string lpName, string lpValue);
        private CallResult<SteamUGCQueryCompleted_t> m_SteamUGCQueryCompleted;
        private Timer steamTimer;

        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ListBoxWorkshopItem.SelectedIndexChanged += new System.EventHandler(this.ListBoxWorkshopItem_SelectedIndexChanged);
            this.BtnUpload.Click -= new System.EventHandler(this.BtnUpload_Click);
            this.BtnUpload.Click += new System.EventHandler(this.BtnUpload_Click);
        }

        private void Form1_Load(object sender, EventArgs e) => ChargerWorkshopSteam();

        private void ChargerWorkshopSteam()
        {
            try
            {
                SetEnvironmentVariable("SteamAppId", APP_ID_CIBLE.ToString());

                if (SteamAPI.Init())
                {
                    if (steamTimer == null)
                    {
                        steamTimer = new Timer();
                        steamTimer.Tick += (s, e) => SteamAPI.RunCallbacks();
                        steamTimer.Start();
                    }
                    m_SteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create(OnWorkshopQueryCompleted);
                    AppId_t jeuCible = new AppId_t(APP_ID_CIBLE);
                    UGCQueryHandle_t handle = SteamUGC.CreateQueryUserUGCRequest(
                    SteamUser.GetSteamID().GetAccountID(),
                    EUserUGCList.k_EUserUGCList_Published,
                    EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items,
                    EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc,
                    AppId_t.Invalid,
                    jeuCible,
                    1
                );

                    SteamAPICall_t apiCall = SteamUGC.SendQueryUGCRequest(handle);
                    m_SteamUGCQueryCompleted.Set(apiCall);
                }
                else
                {
                    MessageBox.Show($"Impossible de se connecter à Steam.\nVérifiez que Steam est lancé en arrière-plan.", "Erreur Steam", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    {
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur SDK Steamworks : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnWorkshopQueryCompleted(SteamUGCQueryCompleted_t callback, bool bIOFailure)
        {
            ListBoxWorkshopItem.Items.Clear();

            if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
            {
                MessageBox.Show("Error Workshop Steam (Code : " + callback.m_eResult + ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (callback.m_unNumResultsReturned > 0)
            {
                for (uint i = 0; i < callback.m_unNumResultsReturned; i++)
                {
                    if (SteamUGC.GetQueryUGCResult(callback.m_handle, i, out SteamUGCDetails_t details))
                    {
                        WorkshopItem item = new WorkshopItem
                        {
                            Title = details.m_rgchTitle,
                            Description = details.m_rgchDescription,
                            Tags = details.m_rgchTags,
                            FileId = details.m_nPublishedFileId
                        };
                        ListBoxWorkshopItem.Items.Add(item);
                    }
                }
            }

            SteamUGC.ReleaseQueryUGCRequest(callback.m_handle);
            MettreAJourTexteBouton();
        }

        private void ListBoxWorkshopItem_SelectedIndexChanged(object sender, EventArgs e) => MettreAJourTexteBouton();

        private void MettreAJourTexteBouton() => BtnUpload.Text = (ListBoxWorkshopItem.SelectedItem != null) ? "Update" : "Upload";

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            Form2 form2;

            if (ListBoxWorkshopItem.SelectedItem is WorkshopItem modSelectionne)
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (steamTimer != null)
            {
                steamTimer.Stop();
                steamTimer.Dispose();
                SteamAPI.Shutdown();
            }
        }

        private void BtnRefreshList_Click(object sender, EventArgs e) => ChargerWorkshopSteam();

        public class WorkshopItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Tags { get; set; }
            public PublishedFileId_t FileId { get; set; }

            public override string ToString()
            {
                string affichageTitre = string.IsNullOrEmpty(Title) ? "NO TITLE!" : Title;
                return $"{affichageTitre}";
            }
        }
        private void upGunToolStripMenuItem_Click(object sender, EventArgs e) => Process.Start("https://discord.gg/9VKrCEbyAV");

        private void startUpGunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // On s'assure que Steam tourne bien en arrière-plan
            if (!SteamAPI.IsSteamRunning())
            {
                MessageBox.Show("Steam doit être lancé pour localiser le jeu.", "Erreur Steam", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // On prépare l'ID de UpGun (1575870)
            AppId_t appIdJeu = new AppId_t(APP_ID_CIBLE);

            // Cette fonction magique interroge Steam pour avoir le dossier exact, peu importe le disque (C:, D:, E:, etc.)
            uint tailleChemin = SteamApps.GetAppInstallDir(appIdJeu, out string dossierInstallation, 1024);

            // Si Steam renvoie un chemin valide et que le dossier existe physiquement
            if (tailleChemin > 0 && System.IO.Directory.Exists(dossierInstallation))
            {
                // On assemble proprement le chemin vers l'exécutable UpGun.exe
                string cheminExe = System.IO.Path.Combine(dossierInstallation, "UpGun.exe");

                // Sécurité : on vérifie que l'exe est bien là avant de tenter un lancement
                if (System.IO.File.Exists(cheminExe))
                {
                    try
                    {
                        // On lance directement l'exécutable local !
                        Process.Start(cheminExe);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Impossible de lancer le fichier exe : {ex.Message}", "Erreur de lancement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"Le dossier du jeu a été trouvé, mais 'UpGun.exe' est introuvable à cet endroit :\n{cheminExe}", "Fichier introuvable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Impossible de détecter le dossier d'installation de UpGun.\nVérifiez que le jeu est bien installé sur ce compte Steam.", "Erreur d'emplacement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
