namespace MinecraftQuery
{
    public class StatusResult : PingResult
    {
        public string MotD { get; internal set; }
        public string Version { get; internal set; }
        public string CurrentPlayers { get; internal set; }
        public string MaxPlayers { get; internal set; }

        internal StatusResult(string address, ushort port) : base(address, port)
        {
        }
    }
}