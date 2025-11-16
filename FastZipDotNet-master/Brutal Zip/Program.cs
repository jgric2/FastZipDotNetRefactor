using Brutal_Zip.Classes;
using Brutal_Zip.Classes.Helpers;
using BrutalZip;
using System.Runtime.InteropServices;

namespace Brutal_Zip
{
    internal static class Program
    {

        static bool IsSingleFile()
        {
            // Reliable in .NET 8: Location returns empty for single-file hosted assemblies
            try
            {
                var loc = System.Reflection.Assembly.GetExecutingAssembly().Location;
                return string.IsNullOrEmpty(loc);
            }
            catch { return false; }
        }


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
            bool iAmPrimary = SingleInstance.TryBecomePrimary();

            if (!iAmPrimary)
            {
                if (SingleInstance.TryForwardToPrimary(args, totalWaitMs: 240, attemptIntervalMs: 60, connectTimeoutMs: 60))
                {
                    return;
                }

                return;
            }
#if NET6_0_OR_GREATER

            ApplicationConfiguration.Initialize();

#else
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
#endif

            SettingsService.Load();
            // Keep server alive for the whole app lifetime (no idle timeout)
            var dispatcher = new SingleInstanceDispatcher();
            SingleInstance.StartServer(dispatcher.OnArgs, idleTimeoutMs: -1);  // -1 => no idle timeout
  
            var form = new MainForm();

            dispatcher.Attach(form);
            form.FormClosed += (_, __) =>
            {
                SingleInstance.StopServer();
            };

            if (args != null && args.Length > 0)
            {
                form.Shown += async (_, __) =>
                {
                    try { await form.HandleCommandAsync(args);  }
                    catch (Exception ex) {}
                };
            }

            Application.Run(form);
        }


        internal sealed class SingleInstanceDispatcher
        {
            private MainForm _form;
            private readonly object _gate = new object();
            private readonly Queue<string[]> _pending = new Queue<string[]>();

            public void OnArgs(string[] args)
            {
                lock (_gate)
                {
                    if (_form == null)
                    {
                        _pending.Enqueue(args);
                        return;
                    }
                }
                _form.BeginInvoke(new Action(async () =>
                {
                    try
                    {
                        await _form.HandleCommandAsync(args);
                    }
                    catch (Exception ex) {  }
                }));
            }

            public void Attach(MainForm form)
            {
                lock (_gate)
                {
                    _form = form;
                    while (_pending.Count > 0)
                    {
                        var a = _pending.Dequeue();
                        _form.BeginInvoke(new Action(async () =>
                        {
                            try
                            {
                                await _form.HandleCommandAsync(a);
                            }
                            catch (Exception ex) {  }
                        }));
                    }
                }
            }
        }

    }




}