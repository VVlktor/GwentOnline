using GwentWebAssembly.Data;
using GwentWebAssembly.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace GwentWebAssembly.Services
{
    public class DeckService : IDeckService
    {
        private readonly HttpClient _httpClient;
        private PlayerService _playerService;

        public DeckService(HttpClient client, PlayerService playerService)
        {
            _playerService = playerService;
            _httpClient = client;
        }

        public async Task<ResponseData> VerifyDeck(PlayerInfo playerInfo)
        {
            string json = JsonSerializer.Serialize(playerInfo);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync($"http://localhost:5277/lobby/SetReady/{_playerService.LobbyCode}", data);
            string stringResponse = await response.Content.ReadAsStringAsync();
            ResponseData responeData = JsonSerializer.Deserialize<ResponseData>(stringResponse);
            return responeData;
        }
    }
}
