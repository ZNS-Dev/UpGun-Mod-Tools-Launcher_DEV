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

namespace UpGun_Mods_Tool_Launcher
{
    public partial class Form2 : Form
    {
        private string SelectFileIcon = "";
        public Form2()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select icon for truc machin chose jsp quoi";
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image File (*.png)|*.png";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SelectFileIcon = openFileDialog.FileName;

                    textBox5.Text = SelectFileIcon;

                    try
                    {
                        string nomFichier = Path.GetFileName(SelectFileIcon);
                        string dossierMods = Path.Combine(Application.StartupPath, "Mods");

                        if (!Directory.Exists(dossierMods))
                        {
                            Directory.CreateDirectory(dossierMods);
                        }

                        string cheminFinal = Path.Combine(dossierMods, nomFichier);
                        File.Copy(SelectFileIcon, cheminFinal, true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) => Form.ActiveForm.Close();
    }
}
