using Newtonsoft.Json;

namespace MinecraftQuery
{
    public struct AccountInfo
    {
        [JsonProperty("id")]
        public string Uuid;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("legacy")]
        public bool Legacy;

        [JsonProperty("demo")]
        public bool Demo;

        public override string ToString() 
            => $"UUID: {Uuid}, Name: {Name}, Legacy: {Legacy}, Demo: {Demo}";
    }
}