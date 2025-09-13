using GwentWebAssembly.Data;
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
        private IGwentHubService _gwentHubService;

        public GameService(HttpClient httpClient, PlayerService playerService, IGwentHubService gwentHubService)
        {
            _httpClient = httpClient;
            _playerService = playerService;
            _gwentHubService = gwentHubService;
        }

        public async Task<StartStatusDto> GetStartStatus()
        {
            var response = await _httpClient.GetAsync($"http://localhost:5277/Game/StartStatus/{_playerService.LobbyCode}/{_playerService.GetIdentity()}");
            string stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true };
            var result = JsonSerializer.Deserialize<StartStatusDto>(stringResponse, options);
            return result;
        }

        public Task CardClicked(GwentBoardCard card)
        {
            throw new NotImplementedException();
        }

        public async Task LaneClicked(GwentLane lane, GwentCard card)
        {
            await _gwentHubService.SendLaneClicked(_playerService.GetIdentity(), _playerService.LobbyCode, lane, card);
        }

        public Task LeaderClicked()
        {
            throw new NotImplementedException();
        }
    }
}
