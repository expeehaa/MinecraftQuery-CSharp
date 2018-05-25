using System;
using System.Linq;

namespace MinecraftQuery
{
    public sealed class MojangService
    {
        public int? Id { get; }
        public string Name { get; }
        public string Servername { get; }

        private static readonly MojangService[] Services;

        public static readonly MojangService MinecraftNet = new MojangService(0, "minecraft.net", "Minecraft.net");
        public static readonly MojangService AccountsWebsite = new MojangService(1, "account.mojang.com", "Mojang Accounts Website");
        public static readonly MojangService Authentication = new MojangService(2, "authserver.mojang.com", "Authentication Service");
        public static readonly MojangService MultiplayerSession = new MojangService(3, "sessionserver.mojang.com", "Multiplayer Session Service");
        public static readonly MojangService MinecraftSkins = new MojangService(4, "textures.minecraft.net", "Minecraft skins");
        public static readonly MojangService PublicApi = new MojangService(5, "api.mojang.com", "Public API");

        static MojangService()
        {
            Services = new[]
            {
                MinecraftNet,
                AccountsWebsite,
                Authentication,
                MultiplayerSession,
                MinecraftSkins,
                PublicApi
            };
        }

        public static MojangService FromServername(string servername) 
            => Services.FirstOrDefault(s => s.Servername.Equals(servername, StringComparison.OrdinalIgnoreCase));


        private MojangService(int id, string servername, string name = "")
        {
            Id = id;
            Name = name;
            Servername = servername;
        }

        internal MojangService(string name)
        {
            Name = name;
        }

        public override string ToString()
            => Name;

        public override bool Equals(object obj) 
            => obj is MojangService ms && Equals(ms);

        public bool Equals(MojangService other) 
            => Id == null || other.Id == null ? Name == other.Name : Id == other.Id;

        public override int GetHashCode() 
            => Id ?? Name.GetHashCode();
    }
}