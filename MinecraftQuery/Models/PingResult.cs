namespace MinecraftQuery
{
    public class PingResult
    {
        public string HostAddress { get; }
        public ushort HostPort { get; }
        public bool ServerAvailable { get; internal set; }
        public long Ping { get; internal set; }

        internal PingResult(string hostAddress, ushort hostPort)
        {
            HostAddress = hostAddress;
            HostPort = hostPort;
            ServerAvailable = false;
        }
    }
}