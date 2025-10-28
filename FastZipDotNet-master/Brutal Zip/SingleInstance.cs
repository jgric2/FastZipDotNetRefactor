using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Brutal_Zip
{
    internal static class SingleInstance
    {
        private static Mutex _mutex;
        private static CancellationTokenSource _cts;
        private static Task _serverTask;
        private static string PipeName => $"BrutalZipPipe_{WindowsIdentity.GetCurrent()?.User?.Value ?? "Default"}";

        public static bool TryBecomePrimary(out bool token)
        {
            bool createdNew;
            _mutex = new Mutex(true, @"Global\BrutalZip_SingleInstance", out createdNew);
            token = createdNew;
            return createdNew;
        }

        public static void StartServer(MainForm form)
        {
            _cts = new CancellationTokenSource();
            _serverTask = Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        using var server = new NamedPipeServerStream(PipeName, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                        await server.WaitForConnectionAsync(_cts.Token).ConfigureAwait(false);

                        using var ms = new System.IO.MemoryStream();
                        var buffer = new byte[4096];
                        do
                        {
                            int r = await server.ReadAsync(buffer, 0, buffer.Length, _cts.Token).ConfigureAwait(false);
                            if (r <= 0) break;
                            ms.Write(buffer, 0, r);
                        } while (!server.IsMessageComplete);

                        var json = Encoding.UTF8.GetString(ms.ToArray());
                        var args = JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();

                        form.BeginInvoke(new Action(async () => await form.HandleCommandAsync(args)));
                    }
                    catch (OperationCanceledException) { break; }
                    catch { /* loop again */ }
                }
            }, _cts.Token);
        }

        public static void StopServer()
        {
            try { _cts?.Cancel(); } catch { }
            try { _serverTask?.Wait(500); } catch { }
            try { _mutex?.ReleaseMutex(); } catch { }
            _cts = null;
            _serverTask = null;
            _mutex = null;
        }

        public static void SendArgsToPrimary(string[] args)
        {
            using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
            client.Connect(1000);
            var json = JsonSerializer.Serialize(args);
            var bytes = Encoding.UTF8.GetBytes(json);
            client.Write(bytes, 0, bytes.Length);
        }
    }
}
