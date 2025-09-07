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

        public async Task<ResponseData> VerifyAndSetDeck(PlayerInfo playerInfo)
        {
            string json = JsonSerializer.Serialize(playerInfo);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync($"http://localhost:5277/Lobby/VerifyAndSetDeck/{_playerService.LobbyCode}/{_playerService.WhichPlayer()}", data);
            string stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            ResponseData responeData = JsonSerializer.Deserialize<ResponseData>(stringResponse, options);
            return responeData;
        }

        public async Task<bool> PlayersReady(string lobbyCode)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5277/lobby/PlayersReady/{lobbyCode}");
            if (!response.IsSuccessStatusCode) return false;
            bool result = bool.Parse(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<PlayerInfo> GetPlayerInfo()
        {
            var response = await _httpClient.GetAsync($"http://localhost:5277/lobby/GetPlayerInfo/{_playerService.LobbyCode}/{_playerService.WhichPlayer()}");
            string responseString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true };
            PlayerInfo responeData = JsonSerializer.Deserialize<PlayerInfo>(responseString, options);
            return responeData;
        }

        public async Task<PlayerInfo> SwapCardInDeck(int id)
        {
            string json = JsonSerializer.Serialize(id);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"http://localhost:5277/Lobby/SwapCard/{_playerService.LobbyCode}/{_playerService.WhichPlayer()}", data);
            string stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true };
            PlayerInfo playerInfo = JsonSerializer.Deserialize<PlayerInfo>(stringResponse, options);
            return playerInfo;
        }

        public async Task<bool> SetReady()
        {
            var response = await _httpClient.GetAsync($"http://localhost:5277/Game/ReadyForGame/{_playerService.LobbyCode}/{_playerService.WhichPlayer()}");
            if (!response.IsSuccessStatusCode) return false;
            bool result = bool.Parse(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<bool> GameReady()
        {
            var response = await _httpClient.GetAsync($"http://localhost:5277/Game/PlayersReady/{_playerService.LobbyCode}");
            if (!response.IsSuccessStatusCode) return false;
            bool result = bool.Parse(await response.Content.ReadAsStringAsync());
            return result;
        }
    }
}
