using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System.Threading.Tasks;

namespace UpGun_Mod_Tools_Launcher.Forms
{
    public static class MessageBox
    {
        public static async Task Show(Window parent, string title, string message)
        {
            var dialog = new Window
            {
                Title = title,
                Width = 380,
                Height = 160,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                SystemDecorations = SystemDecorations.Full
            };

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(20),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var textBlock = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };

            var button = new Button
            {
                Content = "OK",
                Width = 90,
                HorizontalAlignment = HorizontalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            button.Click += (s, e) => dialog.Close();

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(button);
            dialog.Content = stackPanel;

            await dialog.ShowDialog(parent);
        }
    }
}