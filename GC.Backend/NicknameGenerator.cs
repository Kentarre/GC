using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GC.Backend.Interfaces;
using GC.Backend.Models;
using Newtonsoft.Json;

namespace GC.Backend
{
    public class NicknameGenerator : IGenerateNicknames
    {
        private readonly string _remoteUrl;
        private readonly HttpClient _httpClient;

        public NicknameGenerator()
        {
            _remoteUrl = "https://api.namefake.com";
            _httpClient = new HttpClient();

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        public async Task<string> GenerateAsync()
        {
            var request = await _httpClient.GetAsync(_remoteUrl);
            var response = await request.Content.ReadAsStringAsync();
            var serialized = JsonConvert.DeserializeObject<User>(response);

            return $"{serialized.Username}";
        }
    }
}
