using System.Net.Sockets;
using System.Text;

namespace Penaltykick_Game
{
    public class NetClient
    {
        TcpClient tcp = new TcpClient();
        StreamReader? reader;
        StreamWriter? writer;

        public event Action<string>? OnLine;

        public async Task<bool> Connect(string host, int port)
        {
            try
            {
                await tcp.ConnectAsync(host, port);
                var s = tcp.GetStream();
                reader = new StreamReader(s, Encoding.UTF8);
                writer = new StreamWriter(s, Encoding.UTF8) { AutoFlush = true };
                _ = Task.Run(RecvLoop);
                return true;
            }
            catch { return false; }
        }

        async Task RecvLoop()
        {
            try
            {
                while (tcp.Connected)
                {
                    var line = await reader!.ReadLineAsync();
                    if (line == null) break;
                    OnLine?.Invoke(line);
                }
            }
            catch { }
        }

        public Task Send(string msg) => writer!.WriteLineAsync(msg);
    }
}
