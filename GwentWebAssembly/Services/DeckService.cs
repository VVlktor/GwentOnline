using GwentShared.Classes;
using GwentShared.Classes.Dtos;
using GwentWebAssembly.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace GwentWebAssembly.Services
{
    public class DeckService : IDeckService
    {
        private readonly HttpClient _httpClient;
        private IPlayerService _playerService;
        private ILobbySetupService _lobbySetupService;

        public DeckService(HttpClient client, IPlayerService playerService, ILobbySetupService lobbySetupService)
        {
            _httpClient = client;
            _playerService = playerService;
            _lobbySetupService = lobbySetupService;
        }

        public async Task<ResponseData> VerifyAndSetDeck(PlayerDeckInfo playerInfo)
        {
            string json = JsonSerializer.Serialize(playerInfo);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync($"http://localhost:5277/Lobby/VerifyAndSetDeck/{_playerService.LobbyCode}/{_playerService.GetIdentity()}", data);
            string stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            ResponseData responseData = JsonSerializer.Deserialize<ResponseData>(stringResponse, options);
            if (!responseData.IsValid)
                return responseData;

            await _lobbySetupService.JoinBoardAsync();
            await _lobbySetupService.SendLobbyReady();

            return responseData;
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
            var response = await _httpClient.GetAsync($"http://localhost:5277/lobby/GetPlayerInfo/{_playerService.LobbyCode}/{_playerService.GetIdentity()}");
            string responseString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true };
            PlayerInfo responeData = JsonSerializer.Deserialize<PlayerInfo>(responseString, options);
            return responeData;
        }

        public async Task<PlayerInfo> SwapCardInDeck(int id)
        {
            string json = JsonSerializer.Serialize(id);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"http://localhost:5277/Lobby/SwapCard/{_playerService.LobbyCode}/{_playerService.GetIdentity()}", data);
            string stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true };
            PlayerInfo playerInfo = JsonSerializer.Deserialize<PlayerInfo>(stringResponse, options);
            return playerInfo;
        }

        public async Task<bool> SetReady()
        {
            var response = await _httpClient.GetAsync($"http://localhost:5277/Game/ReadyForGame/{_playerService.LobbyCode}/{_playerService.GetIdentity()}");
            if (!response.IsSuccessStatusCode) return false;
            string stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true };
            var result = JsonSerializer.Deserialize<ReadyDto>(stringResponse, options);
            return result.Ready;
        }

        public async Task<bool> GameReady()
        {
            var response = await _httpClient.GetAsync($"http://localhost:5277/Game/PlayersReady/{_playerService.LobbyCode}");
            if (!response.IsSuccessStatusCode) return false;
            string stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true };
            var result = JsonSerializer.Deserialize<ReadyDto>(stringResponse, options);
            return result.Ready;
        }
    }
}
