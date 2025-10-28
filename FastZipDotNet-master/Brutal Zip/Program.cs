using BrutalZip;

namespace Brutal_Zip
{

    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
#if NET6_0_OR_GREATER
            ApplicationConfiguration.Initialize();
#else
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#endif
            SettingsService.Load();

            var form = new MainForm();
            if (args != null && args.Length > 0)
            {
                form.Shown += async (_, __) => { try { await form.HandleCommandAsync(args); } catch { } };
            }

            Application.Run(form);
        }
    }
}


    //internal static class Program
    //{
    //    /// <summary>
    //    ///  The main entry point for the application.
    //    /// </summary>
    //    [STAThread]
    //    static void Main()
    //    {
    //        // To customize application configuration such as set high DPI settings or default font,
    //        // see https://aka.ms/applicationconfiguration.
    //        ApplicationConfiguration.Initialize();
    //        Application.Run(new MainForm());
    //    }
    //}
