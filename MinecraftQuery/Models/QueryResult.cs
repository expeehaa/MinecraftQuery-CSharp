namespace MinecraftQuery
{
    public class QueryResult : StatusResult
    {
        public string HostIp { get; internal set; }
        public string Gametype { get; internal set; }
        public string GameId { get; internal set; }
        public string Plugins { get; internal set; }
        public string Map { get; internal set; }
        public string[] Players { get; internal set; }

        internal QueryResult(string address, ushort port = 25565) : base(address, port)
        {
        }
    }
}