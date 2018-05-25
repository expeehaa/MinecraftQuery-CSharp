using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftQuery
{
    public static class ServerInfo
    {
        private const ushort DataSize = 0x100;
        private const ushort NumFields = 6;

        public static PingResult PingServer(string address, ushort port, int timeout = 6000)
            => PingServerAsync(address, port, timeout).GetAwaiter().GetResult();

        public static async Task<PingResult> PingServerAsync(string address, ushort port, int timeout = 6000)
        {
            var ping = new PingResult(address, port);

            try
            {
                var cancel = new CancellationTokenSource(timeout);
                var pingTest = new Stopwatch();
                var client = new TcpClient();
                pingTest.Start();
                await Task.WhenAny(Task.Delay(-1, cancel.Token), client.ConnectAsync(ping.HostAddress, ping.HostPort)).ConfigureAwait(false);
                pingTest.Stop();
                if (!client.Connected) return ping;
                ping.ServerAvailable = true;
                ping.Ping = pingTest.ElapsedMilliseconds;
            }
            catch (Exception){ /*ignore*/}

            return ping;
        }


        public static StatusResult GetServerStatus(string address, ushort port, int timeout = 6000)
            => GetServerStatusAsync(address, port, timeout).GetAwaiter().GetResult();

        public static async Task<StatusResult> GetServerStatusAsync(string address, ushort port, int timeout = 6000)
        {
            var status = new StatusResult(address, port);
            var streamData = new byte[DataSize];

            var cancel = new CancellationTokenSource(timeout);

            try
            {
                var pingTest = new Stopwatch();
                var client = new TcpClient();
                pingTest.Start();
                await Task.WhenAny(Task.Delay(-1, cancel.Token), client.ConnectAsync(status.HostAddress, status.HostPort)).ConfigureAwait(false);
                pingTest.Stop();
                if (!client.Connected) return status;
                var stream = client.GetStream();
                var msg = new byte[] { 0xFE, 0x01 };
                await stream.WriteAsync(msg, 0, msg.Length, cancel.Token).ConfigureAwait(false);
                await stream.ReadAsync(streamData, 0, DataSize, cancel.Token).ConfigureAwait(false);
                client.Close();

                status.Ping = pingTest.ElapsedMilliseconds;
            }
            catch (Exception)
            {
                return status;
            }

            if (!streamData.Any()) return status;
            var datastring = Encoding.Unicode.GetString(streamData);
            var data = datastring.Split("\u0000\u0000\u0000".ToCharArray());
            if (data == null || data.Length < NumFields) return status;
            status.ServerAvailable = true;
            status.Version = data[2];
            status.MotD = Regex.Replace(data[3], "§([0-9]|[a-f]|[A-F])", "");
            status.CurrentPlayers = data[4];
            status.MaxPlayers = data[5];
            return status;
        }


        public static QueryResult GetQuery(string address, ushort port, int timeout = 6000)
            => GetQueryAsync(address, port, timeout).GetAwaiter().GetResult();

        public static async Task<QueryResult> GetQueryAsync(string address, ushort port, int timeout = 6000)
        {
            var query = new QueryResult(address, port);

            var cancel = new CancellationTokenSource(timeout);
            byte[] byteresponse;
            try
            {
                var pingTest = new Stopwatch();
                var client = new UdpClient();
                pingTest.Start();
                client.Connect(query.HostAddress, query.HostPort);
                pingTest.Stop();

                client.Send(new byte[] {0xFE, 0xFD, 0x09, 0x00, 0x00, 0x00, 0x01}, 7);
                var receive = new Func<UdpClient, Task<byte[]>>(async c =>
                {
                    var resTask = c.ReceiveAsync();
                    await Task.WhenAny(Task.Delay(-1, cancel.Token), resTask).ConfigureAwait(false);
                    return resTask.IsCompleted ? resTask.Result.Buffer : null;
                });
                var handshake = await receive(client).ConfigureAwait(false);
                if (handshake == null) return query;
                var tokenstring = Encoding.Default.GetString(handshake.Skip(5).ToArray());
                var tokennumber = int.Parse(tokenstring);
                var token = BitConverter.GetBytes(tokennumber);
                if (BitConverter.IsLittleEndian) Array.Reverse(token);

                var requestbytes = new byte[] { 0xFE, 0xFD, 0x00, 0x00, 0x00, 0x00, 0x01 }.Concat(token).Concat(new byte[] { 0x00, 0x00, 0x00, 0x00 }).ToArray();
                client.Send(requestbytes, requestbytes.Length);
                byteresponse = await receive(client).ConfigureAwait(false);
                if (byteresponse == null) return query;

                query.Ping = pingTest.ElapsedMilliseconds;
            }
            catch (Exception)
            {
                return query;
            }

            //parse response
            var buf1 = byteresponse.Skip(16).ToArray();
            var resp = Encoding.Default.GetString(buf1).Split("\x00\x01player_\x00\x00");
            var kv = resp[0].Split("\x00").ToList();

            //assign response data to fields
            query.ServerAvailable = true;

            query.MotD = kv[1];
            query.Gametype = kv[3];
            query.GameId = kv[5];
            query.Version = kv[7];
            query.Plugins = kv[9];
            query.Map = kv[11];
            query.CurrentPlayers = kv[13];
            query.MaxPlayers = kv[15];
            query.HostIp = kv[19];
            query.Players = resp[1].Split("\x00").Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            return query;
        }
    }
}