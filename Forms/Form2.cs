using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace UpGun_Mod_Tools_Launcher
{
    public partial class Form2 : Form
    {
        private readonly uint m_AppIdCible;
        private string SelectFileIcon = "";
        private PublishedFileId_t m_FileId;
        private readonly bool estUneCreation = false;
        private Process workerProcess;

        public Form2()
        {
            InitializeComponent();

            // Protection anti-double événement (-= avant +=)
            this.BtnSelectPak.Click -= ChoisirFichierPak_Click;
            this.BtnSelectPak.Click += ChoisirFichierPak_Click;

            this.BtnSelectIcon.Click -= ChoisirIcône_Click;
            this.BtnSelectIcon.Click += ChoisirIcône_Click;

            this.BtnCloseWindowPublish.Click -= BtnCloseWindowPublish_Click;
            this.BtnCloseWindowPublish.Click += BtnCloseWindowPublish_Click;

            this.BtnPublishMod.Click -= BtnPublishMod_Click;
            this.BtnPublishMod.Click += BtnPublishMod_Click;
        }

        public Form2(uint appIdCible, string titreMod, string descriptionMod, string tagsMod, PublishedFileId_t fileId) : this()
        {
            m_AppIdCible = appIdCible;
            m_FileId = fileId;

            textBox2.Text = titreMod;
            textBox3.Text = descriptionMod;

            estUneCreation = (m_FileId == PublishedFileId_t.Invalid);
            BtnPublishMod.Text = estUneCreation ? "Publish Mod" : "Update Mod";

            if (!string.IsNullOrEmpty(tagsMod))
            {
                string[] listeTags = tagsMod.Split(',').Select(t => t.Trim()).ToArray();
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (listeTags.Contains(checkedListBox1.Items[i].ToString().Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        checkedListBox1.SetItemChecked(i, true);
                    }
                }
            }
        }

        private void BtnPublishMod_Click(object sender, EventArgs e)
        {
            // Désactivation immédiate pour contrer les doubles clics
            BtnPublishMod.Enabled = false;

            if (string.IsNullOrEmpty(PathPak.Text) || !File.Exists(PathPak.Text) || Path.GetExtension(PathPak.Text).ToLower() != ".pak")
            {
                MessageBox.Show("Fichier .pak invalide !", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                BtnPublishMod.Enabled = true;
                return;
            }

            if (string.IsNullOrEmpty(textBox2.Text.Trim()))
            {
                MessageBox.Show("Veuillez indiquer un titre.", "Titre manquant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                BtnPublishMod.Enabled = true;
                return;
            }

            List<string> tagsCoches = new List<string>();
            foreach (var item in checkedListBox1.CheckedItems)
            {
                tagsCoches.Add(item.ToString());
            }

            GererEtatInterface(false);

            // Encodage Base64 des arguments
            string titreB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(textBox2.Text.Trim()));
            string descB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(textBox3.Text.Trim()));
            string pakB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(PathPak.Text));
            string iconB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(SelectFileIcon ?? ""));
            string tagsB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(string.Join(",", tagsCoches)));

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Application.ExecutablePath,
                Arguments = $"--worker-publish {m_AppIdCible} {m_FileId.m_PublishedFileId} \"{titreB64}\" \"{descB64}\" \"{pakB64}\" \"{iconB64}\" \"{tagsB64}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            workerProcess = new Process { StartInfo = startInfo };

            workerProcess.OutputDataReceived += (s, argsData) =>
            {
                // Vérification anti ObjectDisposedException
                if (string.IsNullOrEmpty(argsData.Data) || this.IsDisposed || !this.IsHandleCreated) return;

                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (this.IsDisposed) return;

                        if (argsData.Data.StartsWith("PROGRESS:"))
                        {
                            BtnPublishMod.Text = argsData.Data.Replace("PROGRESS:", "");
                        }
                        else if (argsData.Data.StartsWith("SUCCESS:"))
                        {
                            GererEtatInterface(true);
                            string fileIdGenere = argsData.Data.Replace("SUCCESS:", "");

                            if (MessageBox.Show("Publication réussie ! Ouvrir la page ?", "Succès", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                Process.Start("steam://url/CommunityFilePage/" + fileIdGenere);
                            }

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else if (argsData.Data.StartsWith("ERROR:"))
                        {
                            GererEtatInterface(true);
                            MessageBox.Show(argsData.Data.Replace("ERROR:", ""), "Erreur Steam", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
                }
                catch (ObjectDisposedException) { }
            };

            workerProcess.Start();
            workerProcess.BeginOutputReadLine();
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

            if (actif) BtnPublishMod.Text = estUneCreation ? "Publish Mod" : "Update Mod";
        }

        private void ChoisirFichierPak_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Pak file (*.pak)|*.pak" })
            {
                if (ofd.ShowDialog() == DialogResult.OK) PathPak.Text = ofd.FileName;
            }
        }

        private void ChoisirIcône_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Icon file (*.png)|*.png" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    SelectFileIcon = ofd.FileName;
                    textBox5.Text = SelectFileIcon;
                }
            }
        }

        private void BtnCloseWindowPublish_Click(object sender, EventArgs e) => this.Close();
    }

    // =========================================================================
    // WORKER STEAM (Processus isolé)
    // =========================================================================
    public static class SteamWorkerTask
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetEnvironmentVariable(string lpName, string lpValue);

        private static CallResult<SteamUGCQueryCompleted_t> m_SteamUGCQueryCompleted;
        private static CallResult<CreateItemResult_t> m_CreateItem;
        private static CallResult<SubmitItemUpdateResult_t> m_SubmitItemUpdate;

        private static UGCUpdateHandle_t m_CurrentUpdateHandle;
        private static bool enExecution = true;
        private static PublishedFileId_t fileIdTraite;

        public static void ExecuterRechercheWorkshop(string[] args)
        {
            if (args.Length < 2) return;

            uint appId = uint.Parse(args[1]);
            SetEnvironmentVariable("SteamAppId", appId.ToString());

            if (!SteamAPI.Init()) return;

            m_SteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create((callback, bIOFailure) =>
            {
                if (!bIOFailure && callback.m_eResult == EResult.k_EResultOK)
                {
                    for (uint i = 0; i < callback.m_unNumResultsReturned; i++)
                    {
                        if (SteamUGC.GetQueryUGCResult(callback.m_handle, i, out SteamUGCDetails_t details))
                        {
                            string desc = details.m_rgchDescription.Replace("\r\n", "<BR>").Replace("\n", "<BR>");
                            Console.WriteLine($"{details.m_nPublishedFileId}|{details.m_rgchTitle}|{desc}|{details.m_rgchTags}");
                        }
                    }
                    SteamUGC.ReleaseQueryUGCRequest(callback.m_handle);
                }
                enExecution = false;
            });

            UGCQueryHandle_t handle = SteamUGC.CreateQueryUserUGCRequest(
                SteamUser.GetSteamID().GetAccountID(),
                EUserUGCList.k_EUserUGCList_Published,
                EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items,
                EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc,
                AppId_t.Invalid,
                new AppId_t(appId),
                1
            );

            SteamAPICall_t apiCall = SteamUGC.SendQueryUGCRequest(handle);
            m_SteamUGCQueryCompleted.Set(apiCall);

            while (enExecution)
            {
                SteamAPI.RunCallbacks();
                Thread.Sleep(50);
            }

            SteamAPI.Shutdown();
        }

        public static void ExecuterPublicationWorkshop(string[] args)
        {
            if (args.Length < 8) return;

            uint appId = uint.Parse(args[1]);
            fileIdTraite = new PublishedFileId_t(ulong.Parse(args[2]));
            string titre = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[3]));
            string desc = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[4]));
            string pakPath = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[5]));
            string iconPath = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[6]));
            string tags = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[7]));

            SetEnvironmentVariable("SteamAppId", appId.ToString());

            if (!SteamAPI.Init())
            {
                Console.WriteLine("ERROR:Impossible de se connecter à l'API Steam.");
                return;
            }

            m_CreateItem = CallResult<CreateItemResult_t>.Create((callback, bIOFailure) =>
            {
                if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
                {
                    Console.WriteLine("ERROR:Échec de création sur Steam. Code: " + callback.m_eResult);
                    enExecution = false;
                    return;
                }

                fileIdTraite = callback.m_nPublishedFileId;
                DemarrerUpdate(appId, fileIdTraite, titre, desc, pakPath, iconPath, tags);
            });

            m_SubmitItemUpdate = CallResult<SubmitItemUpdateResult_t>.Create((callback, bIOFailure) =>
            {
                if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
                {
                    Console.WriteLine("ERROR:Échec de l'envoi Steam. Code: " + callback.m_eResult);
                }
                else
                {
                    Console.WriteLine("SUCCESS:" + fileIdTraite.m_PublishedFileId);
                }
                enExecution = false;
            });

            if (fileIdTraite == PublishedFileId_t.Invalid)
            {
                Console.WriteLine("PROGRESS:Création du Mod sur Steam...");
                SteamAPICall_t apiCall = SteamUGC.CreateItem(new AppId_t(appId), EWorkshopFileType.k_EWorkshopFileTypeCommunity);
                m_CreateItem.Set(apiCall);
            }
            else
            {
                DemarrerUpdate(appId, fileIdTraite, titre, desc, pakPath, iconPath, tags);
            }

            while (enExecution)
            {
                SteamAPI.RunCallbacks();

                if (m_CurrentUpdateHandle != UGCUpdateHandle_t.Invalid)
                {
                    EItemUpdateStatus status = SteamUGC.GetItemUpdateProgress(m_CurrentUpdateHandle, out ulong bytesProcessed, out ulong bytesTotal);
                    string textStatus = "Préparation...";

                    switch (status)
                    {
                        case EItemUpdateStatus.k_EItemUpdateStatusPreparingConfig: textStatus = "Configuration..."; break;
                        case EItemUpdateStatus.k_EItemUpdateStatusPreparingContent: textStatus = "Préparation des fichiers..."; break;
                        case EItemUpdateStatus.k_EItemUpdateStatusUploadingContent: textStatus = "Envoi du contenu..."; break;
                        case EItemUpdateStatus.k_EItemUpdateStatusUploadingPreviewFile: textStatus = "Envoi de l'icône..."; break;
                        case EItemUpdateStatus.k_EItemUpdateStatusCommittingChanges: textStatus = "Validation..."; break;
                    }

                    if (bytesTotal > 0)
                    {
                        int progress = (int)((bytesProcessed * 100) / bytesTotal);
                        Console.WriteLine($"PROGRESS:{textStatus} ({progress}%)");
                    }
                    else
                    {
                        Console.WriteLine($"PROGRESS:{textStatus}");
                    }
                }

                Thread.Sleep(100);
            }

            SteamAPI.Shutdown();
        }

        private static void DemarrerUpdate(uint appId, PublishedFileId_t fileId, string titre, string desc, string pakPath, string iconPath, string tags)
        {
            AppId_t appIdObj = new AppId_t(appId);
            m_CurrentUpdateHandle = SteamUGC.StartItemUpdate(appIdObj, fileId);

            SteamUGC.SetItemTitle(m_CurrentUpdateHandle, titre);
            SteamUGC.SetItemDescription(m_CurrentUpdateHandle, desc);

            string dossierEnvoi = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DONT_DELETE");
            if (Directory.Exists(dossierEnvoi))
            {
                try { Directory.Delete(dossierEnvoi, true); } catch { }
            }
            Directory.CreateDirectory(dossierEnvoi);

            string nomPak = Path.GetFileName(pakPath);
            File.Copy(pakPath, Path.Combine(dossierEnvoi, nomPak), true);

            SteamUGC.SetItemContent(m_CurrentUpdateHandle, dossierEnvoi);

            if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
            {
                SteamUGC.SetItemPreview(m_CurrentUpdateHandle, iconPath);
            }

            if (!string.IsNullOrEmpty(tags))
            {
                List<string> listTags = tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                SteamUGC.SetItemTags(m_CurrentUpdateHandle, listTags);
            }

            SteamAPICall_t apiCall = SteamUGC.SubmitItemUpdate(m_CurrentUpdateHandle, "Mise à jour");
            m_SubmitItemUpdate.Set(apiCall);
        }
    }
}