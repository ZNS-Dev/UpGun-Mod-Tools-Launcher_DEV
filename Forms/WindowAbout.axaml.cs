using Avalonia.Controls;
using Avalonia.Interactivity;

namespace UpGun_Mod_Tools_Launcher.Forms
{
    public partial class WindowAbout : Window
    {
        public WindowAbout()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}