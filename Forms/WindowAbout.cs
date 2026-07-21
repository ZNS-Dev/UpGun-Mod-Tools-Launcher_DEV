using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpGun_Mod_Tools_Launcher.Forms
{
    public partial class WindowAbout : Form
    {
        public WindowAbout()
        {
            InitializeComponent();
        }

        private void BtnAboutOk_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}