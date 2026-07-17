using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpGun_Mods_Tool_Launcher
{
    public partial class Form1 : Form
    {
        private string SelectFilePak = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void BtnStartUpGun_Click(object sender, EventArgs e)
        {
            Process.Start("steam://rungameid/1575870");
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select pak to upload";
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Pak File (*.pak)|*.pak";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SelectFilePak = openFileDialog.FileName;

                    textBox1.Text = SelectFilePak;

                    try
                    {
                        string nomFichier = Path.GetFileName(SelectFilePak);
                        string dossierMods = Path.Combine(Application.StartupPath, "Mods");

                        if (!Directory.Exists(dossierMods))
                        {
                            Directory.CreateDirectory(dossierMods);
                        }

                        string cheminFinal = Path.Combine(dossierMods, nomFichier);
                        File.Copy(SelectFilePak, cheminFinal, true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void BtnUpload_Click_1(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }
    }
}