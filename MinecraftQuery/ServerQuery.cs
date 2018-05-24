/*
 * Copyright expeehaa
 */

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MinecraftQuery
{
    public class ServerQuery
    {
        public bool ServerAvailable { get; set; }
        public string MotD { get; set; }
        public string Gametype { get; set; }
        public string GameId { get; set; }
        public string Version { get; set; }
        public string Plugins { get; set; }
        public string Map { get; set; }
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public short HostPort { get; set; }
        public string HostIp { get; set; }
        public string[] Players { get; set; }

        public ServerQuery(string host, short port = 25565) {
            var addresses = Dns.GetHostAddresses(host);
            if (addresses.Length == 0)
                ServerAvailable = false;
            Init(addresses[0], port);
        }

        public ServerQuery(IPAddress ipAddress, short port = 25565) {
            Init(ipAddress, port);
        }

        private void Init(IPAddress ipAddress, short port) {
            //initialize UdpClient
            var ipendpoint = new IPEndPoint(ipAddress, port);
            byte[] byteresponse;
            try {
                using (var client = IPAddress.IsLoopback(ipAddress) ? new UdpClient("localhost", port) : new UdpClient(ipendpoint)) {
                    client.Send(new byte[] { 0xFE, 0xFD, 0x09, 0x00, 0x00, 0x00, 0x01 }, 7);
                    //receive handshake
                    var handshake = client.Receive(ref ipendpoint);
                    //parse token
                    var tokenstring = Encoding.Default.GetString(handshake.Skip(5).ToArray());
                    var tokennumber = int.Parse(tokenstring);
                    var token = BitConverter.GetBytes(tokennumber);
                    if (BitConverter.IsLittleEndian) Array.Reverse(token);
                    //send full stat request
                    var requestbytes = new byte[] { 0xFE, 0xFD, 0x00, 0x00, 0x00, 0x00, 0x01 }.Concat(token).Concat(new byte[] { 0x00, 0x00, 0x00, 0x00 }).ToArray();
                    client.Send(requestbytes, requestbytes.Length);
                    //receive full stat response
                    byteresponse = client.Receive(ref ipendpoint);
                }
            }
            catch (Exception) {
                ServerAvailable = false;
                return;
            }

            //parse response
            var buf1 = byteresponse.Skip(16).ToArray();
            var resp = Encoding.Default.GetString(buf1).Split("\x00\x01player_\x00\x00");
            var kv = resp[0].Split("\x00").ToList();

            //assign response data to fields
            ServerAvailable = true;

            MotD = kv[1];
            Gametype = kv[3];
            GameId = kv[5];
            Version = kv[7];
            Plugins = kv[9];
            Map = kv[11];
            CurrentPlayers = int.Parse(kv[13]);
            MaxPlayers = int.Parse(kv[15]);
            HostPort = short.Parse(kv[17]);
            HostIp = kv[19];

            Players = resp[1].Split("\x00").Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
        }
    }
}