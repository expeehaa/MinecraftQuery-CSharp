using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace MinecraftQuery
{
    public class ServerPinger
    {
        private const ushort DataSize = 0x200;
        private const ushort NumFields = 6;

        public bool ServerAvailable { get; set; }
        public string MotD { get; set; }
        public string HostAddress { get; set; }
        public ushort HostPort { get; set; }
        public string Version { get; set; }
        public string CurrentPlayers { get; set; }
        public string MaxPlayers { get; set; }
        public long Ping { get; set; }

        public ServerPinger(string address, ushort port)
        {
            var streamData = new byte[DataSize];

            HostAddress = address;
            HostPort = port;

            ServerAvailable = false;
            try
            {
                var pingTest = new Stopwatch();
                var client = new TcpClient();
                pingTest.Start();
                client.Connect(address, port);
                pingTest.Stop();
                var stream = client.GetStream();
                var msg = new byte[] { 0xFE, 0x01 };
                stream.Write(msg, 0, msg.Length);
                stream.Read(streamData, 0, DataSize);
                client.Close();
                Ping = pingTest.ElapsedMilliseconds;
            }
            catch (Exception)
            {
                return;
            }

            if (!streamData.Any()) return;
            var datastring = Encoding.Unicode.GetString(streamData);
            var data = datastring.Split("\u0000\u0000\u0000".ToCharArray());
            if (data == null || data.Length < NumFields) return;
            ServerAvailable = true;
            Version = data[2];
            MotD = Regex.Replace(data[3], "§([0-9]|[a-f]|[A-F])", "");
            CurrentPlayers = data[4];
            MaxPlayers = data[5];
        }
    }
}