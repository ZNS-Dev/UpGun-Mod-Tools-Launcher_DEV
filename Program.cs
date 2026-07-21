using Avalonia;
using System;
using System.Diagnostics;

namespace UpGun_Mod_Tools_Launcher
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Conserver le comportement Worker CLI
            if (args.Length > 0 && args[0] == "--worker-query")
            {
                SteamWorkerTask.ExecuteWorkshopSearch(args);
                return;
            }
            if (args.Length > 0 && args[0] == "--worker-publish")
            {
                SteamWorkerTask.ExecuteWorkshopPublish(args);
                return;
            }

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}