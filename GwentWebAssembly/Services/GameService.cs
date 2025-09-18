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

        public async Task JoinBoardAsync()
        {
            await _gwentHubService.JoinBoardAsync(_playerService.LobbyCode);
        }

        public async Task<StartStatusDto> GetStartStatus()
        {
            var response = await _httpClient.GetAsync($"http://localhost:5277/Game/StartStatus/{_playerService.LobbyCode}/{_playerService.GetIdentity()}");
            string stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true };
            var result = JsonSerializer.Deserialize<StartStatusDto>(stringResponse, options);
            return result;
        }

        public async Task CardClicked(GwentBoardCard clickedCard, GwentCard card)
        {
            //tylko decoy
            if (card.CardId != 2) return;

        }

        public Task LeaderClicked()
        {
            throw new NotImplementedException();
        }

        public async Task PlayerLaneClicked(GwentLane lane, GwentCard card)
        {
            if (card.Abilities.HasFlag(Abilities.Spy) ||
               card.Placement == TroopPlacement.Weather ||
               card.Placement == TroopPlacement.Special) return;

            await _gwentHubService.SendLaneClicked(_playerService.GetIdentity(), _playerService.LobbyCode, lane, card);
        }

        public async Task HornClicked(TroopPlacement placement, GwentCard card)
        {
            if (card.CardId == 6)
                await _gwentHubService.SendHornClicked(_playerService.GetIdentity(), _playerService.LobbyCode, placement, card);
        }
    }
}
