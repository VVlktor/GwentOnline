using GwentWebAssembly.Data.Dtos;
using GwentWebAssembly.Services.Interfaces;
using System.Net.Http;
using System.Text.Json;

namespace GwentWebAssembly.Services
{
    public class GameService : IGameService
    {
        private HttpClient _httpClient;
        private PlayerService _playerService;

        public GameService(HttpClient httpClient, PlayerService playerService)
        {
            _httpClient = httpClient;
            _playerService = playerService;
        }

        public async Task<StartStatusDto> GetStartStatus()
        {
            var response = await _httpClient.GetAsync($"http://localhost:5277/Game/StartStatus/{_playerService.LobbyCode}/{_playerService.GetIdentity()}");
            string stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true };
            var result = JsonSerializer.Deserialize<StartStatusDto>(stringResponse, options);
            return result;
        }

    }
}
