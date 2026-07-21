using System;
using System.Windows.Forms;

namespace UpGun_Mod_Tools_Launcher
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                if (args[0] == "--worker-query")
                {
                    SteamWorkerTask.ExecuteWorkshopSearch(args);
                    return;
                }
                else if (args[0] == "--worker-publish")
                {
                    SteamWorkerTask.ExecuteWorkshopSearch(args);
                    return;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}