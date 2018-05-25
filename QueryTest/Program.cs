using System;
using System.Linq;
using MinecraftQuery;

namespace QueryTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var mapi = new MojangApi();
            var xph = mapi.GetAccountInfo("expeehaa");
            Console.WriteLine(xph.ToString());
            var xphnames = mapi.GetAllAccountNames(xph.Uuid);
            Console.WriteLine(xphnames.ToString());
            var servicestatus = mapi.GetServiceStatus();
            Console.WriteLine(string.Join(", ", servicestatus.Select(kv => $"{kv.Key.Name}: {kv.Value}")));

            const string address = "localhost";
            const ushort port = 25565;
            var pr = ServerInfo.PingServer(address, port);
            var st = ServerInfo.GetServerStatus(address, port);
            var qu = ServerInfo.GetQuery(address, port);
            Console.WriteLine($"====== Ping ======\nServerstatus: {(st.ServerAvailable ? "Online" : "Offline")}\nPing: {st.Ping}\n");
            Console.WriteLine($"\n====== Status ======\nServerstatus: {(st.ServerAvailable ? "Online" : "Offline")}\nPing: {st.Ping}\nMotD: {st.MotD}\nPlayers: {st.CurrentPlayers}/{st.MaxPlayers}");
            Console.WriteLine($"\n====== Query ======\nServerstatus: {(st.ServerAvailable ? "Online" : "Offline")}\nPing: {qu.Ping}\nMotD: {qu.MotD}\nPlayers: {qu.CurrentPlayers}/{qu.MaxPlayers}\nMap: {qu.Map}");
            Console.ReadLine();
        }
    }
}
