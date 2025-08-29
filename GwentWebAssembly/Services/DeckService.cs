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
            _httpClient = client;
            _playerService = playerService;
        }

        public async Task<ResponseData> VerifyDeck(PlayerInfo playerInfo)
        {
            string json = JsonSerializer.Serialize(playerInfo);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync($"http://localhost:5277/Lobby/VerifyDeck", data);
            string stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };
            ResponseData responeData = JsonSerializer.Deserialize<ResponseData>(stringResponse, options);
            return responeData;
        }

        public async Task SetDeck(PlayerInfo playerInfo)
        {
            string json = JsonSerializer.Serialize<PlayerInfo>(playerInfo);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            int player = _playerService.WhichPlayer() == PlayerIdentity.PlayerOne ? 1 : 2;
            await _httpClient.PostAsync($"http://localhost:5277/Lobby/SetDeck/{_playerService.LobbyCode}/{player}", data);
        }

        public async Task<bool> PlayersReady(string lobbyCode)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5277/lobby/PlayersReady/{lobbyCode}");
            if (!response.IsSuccessStatusCode) return false;
            bool result = bool.Parse(await response.Content.ReadAsStringAsync());
            return result;
        }
    }
}
