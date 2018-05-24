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
            var xph = mapi.GetAccountInfoAsync("expeehaa").GetAwaiter().GetResult();
            Console.WriteLine(xph.ToString());
            var xphnames = mapi.GetAllAccountNamesAsync(xph.Uuid).GetAwaiter().GetResult();
            Console.WriteLine(xphnames.ToString());
            var servicestatus = mapi.GetServiceStatus().GetAwaiter().GetResult();
            Console.WriteLine(string.Join(", ", servicestatus.Select(kv => $"{kv.Key.Name}: {kv.Value}")));
            //var mq = new ServerQuery("");
            //Console.WriteLine($"Online: {mq.ServerAvailable}\nMotD: {mq.MotD}\nPlayers: {mq.CurrentPlayers}/{mq.MaxPlayers}");
            var sp = new ServerPinger("gommehd.net", 25565);
            Console.WriteLine($"Serverstatus: {(sp.ServerAvailable ? "Online" : "Offline")}\nPing: {sp.Ping}\nMotD: {sp.MotD}\nPlayers: {sp.CurrentPlayers}/{sp.MaxPlayers}");

            Console.ReadKey();
        }
    }
}
