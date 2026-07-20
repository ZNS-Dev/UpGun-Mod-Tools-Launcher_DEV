using Avalonia;
using System;
using UpGun_Mod_Tools_Launcher.Steam;

namespace UpGun_Mod_Tools_Launcher
{
    internal static class Program
    {
        // Point d'entrée natif. IMPORTANT : ne rien mettre ici qui touche Avalonia
        // avant BuildAvaloniaApp (évite d'initialiser des sous-systèmes graphiques
        // inutilement dans les process "worker" headless).
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "--worker-query":
                        SteamWorkerTask.ExecuterRechercheWorkshop(args);
                        return;
                    case "--worker-publish":
                        SteamWorkerTask.ExecuterPublicationWorkshop(args);
                        return;
                }
            }

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
