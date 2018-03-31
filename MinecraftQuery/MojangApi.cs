using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MinecraftQuery.Models;
using Newtonsoft.Json;

namespace MinecraftQuery
{
    public class MojangApi
    {
        private HttpClient _httpClient;

        public MojangApi()
        {
            InitHttpClient();
        }

        private void InitHttpClient()
        {
            _httpClient = new HttpClient {BaseAddress = new Uri("https://api.mojang.com/")};
        }

        public async Task<AccountInfo> GetAccountInfoAsync(string username, DateTime? date = null)
        {
            var hrm = await _httpClient.GetAsync($"/users/profiles/minecraft/{username}{(date == null ? "" : $"?at={date.Value.ToUnixTime()}")}").ConfigureAwait(false);
            if(hrm.StatusCode == HttpStatusCode.NoContent) throw new ArgumentException($"No account with name \"{username}\" was found!", nameof(username));
            if(hrm.StatusCode == HttpStatusCode.BadRequest) throw new ArgumentOutOfRangeException(nameof(date));
            hrm.EnsureSuccessStatusCode();
            var json = await hrm.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<AccountInfo>(json);
        }

        public async Task<AccountNames> GetAllAccountNamesAsync(string uuid)
        {
            var hrm = await _httpClient.GetAsync($"/user/profiles/{uuid}/names");
            if(hrm.StatusCode == HttpStatusCode.NoContent) throw new ArgumentException($"An account with UUID \"{uuid}\" does not exist!", nameof(uuid));
            hrm.EnsureSuccessStatusCode();
            var json = await hrm.Content.ReadAsStringAsync().ConfigureAwait(false);
            var data = JsonConvert.DeserializeObject<List<JsonNameTime>>(json);
            var an = new AccountNames
            {
                FirstName = data.First().Name
            };

            foreach (var jnt in data.Skip(1).ToList())
                an.TryAdd(jnt.Time, jnt.Name);
            return an;
        }

        public class JsonNameTime
        {
            [JsonProperty("name")]
            public string Name;

            [JsonProperty("changedToAt")]
            public long UnixTime;

            public DateTime Time => UnixTime.ToDateTime();
        }
    }
}