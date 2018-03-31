using System;
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
            Console.ReadKey();
        }
    }
}
