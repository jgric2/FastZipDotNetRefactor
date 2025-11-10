using Brutal_Zip.Classes;
using BrutalZip;
using System.Runtime.InteropServices;

namespace Brutal_Zip
{
    internal static class Program
    {
        class CaretHygieneFilter : IMessageFilter 
        { 
            private const int WM_SETFOCUS = 0x0007; 
            [DllImport("user32.dll")] 
            private static extern bool DestroyCaret(); 
            public bool PreFilterMessage(ref Message m) 
            { 
                if (m.Msg == WM_SETFOCUS) 
                { 
                    try 
                    { 
                        DestroyCaret(); 
                    } catch { } 
                } 
                
                return false; 
            } 
        }

        [STAThread]
        static void Main(string[] args)
        {


            //var form11 = new MainForm3();
            //Application.Run(form11);
            //return;

#if NET6_0_OR_GREATER
            ApplicationConfiguration.Initialize();
#else
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
#endif
            SettingsService.Load();


            Application.AddMessageFilter(new CaretHygieneFilter());

            // 1) Best case: a server is already up – try to forward and exit
            if (SingleInstance.TryForwardToPrimary(args, totalWaitMs: 400, attemptIntervalMs: 80, connectTimeoutMs: 150))
                return;

            // 2) Try to become the primary (non-blocking)
            bool iAmPrimary = SingleInstance.TryBecomePrimary();

            if (!iAmPrimary)
            {
                // 2a) Another process is becoming primary; retry forwarding briefly until it’s ready
                if (SingleInstance.TryForwardToPrimary(args, totalWaitMs: 1500, attemptIntervalMs: 100, connectTimeoutMs: 200))
                    return;

                // Still couldn’t forward; safest is to exit to avoid extra windows in the burst
                return;
            }

            // 3) We are the primary: start server immediately (before UI) to catch the burst
            var form = new MainForm();
            SingleInstance.StartServer(form, idleTimeoutMs: 1200);
            form.FormClosed += (_, __) => SingleInstance.StopServer();

            // Handle our own initial args
            if (args != null && args.Length > 0)
            {
                form.Shown += async (_, __) =>
                {
                    try { await form.HandleCommandAsync(args); } catch { }
                };
            }

            Application.Run(form);
        }
    }
}