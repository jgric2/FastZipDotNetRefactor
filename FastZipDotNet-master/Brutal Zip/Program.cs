using Brutal_Zip.Classes;
using Brutal_Zip.Classes.Helpers;
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


        //TODO: MAKE CUSTOM DETECTOR WITHOUT SINGLE INSTANCE SERVER HAS TO BE A FASTER WAY. NVR MIND WAS JUST WINDOWS BEING STUPID

        [STAThread]
        static void Main(string[] args)
        {
            //Brutal_Zip.Classes.Helpers.BootTrace.Init();
            //BootTrace.Mark("Main entered. Raw args: " + string.Join(" | ", args ?? Array.Empty<string>()));
          //  NativeBootstrap.Initialize();
            //// 1) Try to become primary first (fast)
            //BootTrace.Mark("TryBecomePrimary begin");
            bool iAmPrimary = SingleInstance.TryBecomePrimary();
            //BootTrace.Mark("TryBecomePrimary result: " + iAmPrimary);

            if (!iAmPrimary)
            {
                // 2) Not primary -> forward quickly and exit
               // BootTrace.Mark("Forward attempt begin");
                if (SingleInstance.TryForwardToPrimary(args, totalWaitMs: 240, attemptIntervalMs: 60, connectTimeoutMs: 60))
                {
                 //   BootTrace.Mark("Forward success -> exit");
                    return;
                }
              //  BootTrace.Mark("Forward failed (no ready primary) -> exit");
                return;
            }
#if NET6_0_OR_GREATER
           // BootTrace.Mark("ApplicationConfiguration.Initialize begin");
            ApplicationConfiguration.Initialize();
          // BootTrace.Mark("ApplicationConfiguration.Initialize end");
#else
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
#endif

           // BootTrace.Mark("SettingsService.Load begin");
            SettingsService.Load();
          //  BootTrace.Mark("SettingsService.Load end");

            // Keep server alive for the whole app lifetime (no idle timeout)
            var dispatcher = new SingleInstanceDispatcher();
           // BootTrace.Mark("StartServer(delegate, keepAlive) begin");
            SingleInstance.StartServer(dispatcher.OnArgs, idleTimeoutMs: -1);  // -1 => no idle timeout
          //  BootTrace.Mark("StartServer(delegate, keepAlive) end");

           // BootTrace.Mark("MainForm ctor begin");
            var form = new MainForm();
          //  BootTrace.Mark("MainForm ctor end");

            dispatcher.Attach(form);
            form.FormClosed += (_, __) =>
            {
               // BootTrace.Mark("MainForm closed -> StopServer");
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

           // BootTrace.Mark("Application.Run begin");
            Application.Run(form);
           // BootTrace.Mark("Application.Run end");
        }




        internal sealed class SingleInstanceDispatcher
        {
            private MainForm _form;
            private readonly object _gate = new object();
            private readonly Queue<string[]> _pending = new Queue<string[]>();

            public void OnArgs(string[] args)
            {
               // Brutal_Zip.Classes.Helpers.BootTrace.Mark("Server: onArgs received: " + string.Join(" | ", args ?? Array.Empty<string>()));
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
                       // Brutal_Zip.Classes.Helpers.BootTrace.Mark("Server: dispatch to form -> HandleCommandAsync begin");
                        await _form.HandleCommandAsync(args);
                       // Brutal_Zip.Classes.Helpers.BootTrace.Mark("Server: dispatch to form -> HandleCommandAsync end");
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
                               // Brutal_Zip.Classes.Helpers.BootTrace.Mark("Server: flush pending -> HandleCommandAsync begin");
                                await _form.HandleCommandAsync(a);
                              //  Brutal_Zip.Classes.Helpers.BootTrace.Mark("Server: flush pending -> HandleCommandAsync end");
                            }
                            catch (Exception ex) {  }
                        }));
                    }
                }
            }
        }

    }




}