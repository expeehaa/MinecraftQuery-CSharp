using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MinecraftQuery
{
    public sealed class MojangApi
    {
        private HttpClient _httpClient;

        public MojangApi()
        {
            InitHttpClient();
        }

        private void InitHttpClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.mojang.com/"),
                Timeout = TimeSpan.FromSeconds(6)
            };
        }

        public AccountInfo GetAccountInfo(string username, DateTime? date = null)
            => GetAccountInfoAsync(username, date).GetAwaiter().GetResult();

        public async Task<AccountInfo> GetAccountInfoAsync(string username, DateTime? date = null)
        {
            var hrm = await _httpClient.GetAsync($"/users/profiles/minecraft/{username}{(date == null ? "" : $"?at={date.Value.ToUnixTime()}")}").ConfigureAwait(false);
            if(hrm.StatusCode == HttpStatusCode.NoContent) throw new ArgumentException($"No account with name \"{username}\" was found!", nameof(username));
            if(hrm.StatusCode == HttpStatusCode.BadRequest) throw new ArgumentOutOfRangeException(nameof(date));
            hrm.EnsureSuccessStatusCode();
            var json = await hrm.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<AccountInfo>(json);
        }


        public AccountNames GetAllAccountNames(string uuid)
            => GetAllAccountNamesAsync(uuid).GetAwaiter().GetResult();

        public async Task<AccountNames> GetAllAccountNamesAsync(string uuid)
        {
            var hrm = await _httpClient.GetAsync($"/user/profiles/{uuid}/names");
            if(hrm.StatusCode == HttpStatusCode.NoContent) throw new ArgumentException($"An account with UUID \"{uuid}\" does not exist!", nameof(uuid));
            hrm.EnsureSuccessStatusCode();
            var json = await hrm.Content.ReadAsStringAsync().ConfigureAwait(false);
            var data = JsonConvert.DeserializeObject<List<JsonNameTime>>(json);
            return new AccountNames(data.ToDictionary(jnt => jnt.UnixTime == 0 ? DateTime.MinValue : jnt.Time, jnt => jnt.Name));
        }


        public Dictionary<MojangService, ServiceStatus> GetServiceStatus()
            => GetServiceStatusAsync().GetAwaiter().GetResult();

        public async Task<Dictionary<MojangService, ServiceStatus>> GetServiceStatusAsync()
        {
            var hrm = await _httpClient.GetAsync("https://status.mojang.com/check").ConfigureAwait(false);
            hrm.EnsureSuccessStatusCode();
            var jsonstring = await hrm.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(jsonstring).Select(d => d.First()).ToList();
            var results = new Dictionary<MojangService, ServiceStatus>();
            foreach (var kv in json)
            {
                var ms = MojangService.FromServername(kv.Key) ?? new MojangService(kv.Key);
                if (Enum.TryParse(kv.Value, true, out ServiceStatus stat))
                    results.Add(ms, stat);
            }

            return results;
        } 

        private class JsonNameTime
        {

#pragma warning disable 0649

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("changedToAt")]
            public long UnixTime { get; set; }

#pragma warning restore 0649

            public DateTime Time => UnixTime.ToDateTime();
        }
    }
}