using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace UpGun_Mod_Tools_Launcher.Views
{
    public partial class MessageDialog : Window
    {
        public MessageDialog()
        {
            InitializeComponent();
        }

        public MessageDialog(string title, string message) : this()
        {
            TxtTitle.Text = title;
            TxtMessage.Text = message;
        }

        private void DragArea_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) BeginMoveDrag(e);
        }

        private void BtnOk_Click(object? sender, RoutedEventArgs e) => Close();
    }
}
