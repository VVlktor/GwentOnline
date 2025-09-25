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
        private IStatusService _statusService;
        private IGwentHubService _gwentHubService;

        public GameService(HttpClient httpClient, PlayerService playerService, IGwentHubService gwentHubService, IStatusService statusService)
        {
            _httpClient = httpClient;
            _playerService = playerService;
            _gwentHubService = gwentHubService;
            _statusService = statusService;
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
            await _statusService.InitializeAsync(result);
            return result;
        }

        public async Task CardClicked(GwentBoardCard clickedCard)
        {
            //tylko decoy
            GwentCard card = _statusService.GetSelectedCard();
            if (card.CardId != 2) return;

            await _gwentHubService.SendCardClicked(_playerService.GetIdentity(), _playerService.LobbyCode, clickedCard, card);
        }

        public async Task LeaderClicked()
        {
            throw new NotImplementedException();
        }

        public async Task PlayerLaneClicked(TroopPlacement placement)
        {
            GwentCard card = _statusService.GetSelectedCard();
            if (card is null) return;

            if (card.Abilities.HasFlag(Abilities.Spy) ||
               card.Placement == TroopPlacement.Weather ||
               card.Placement == TroopPlacement.Special) return;

            await _gwentHubService.SendLaneClicked(_playerService.GetIdentity(), _playerService.LobbyCode, placement, card);
        }

        public async Task HornClicked(TroopPlacement placement)
        {
            GwentCard card = _statusService.GetSelectedCard();
            if (card is null) return;
            if (card.CardId == 6)
                await _gwentHubService.SendHornClicked(_playerService.GetIdentity(), _playerService.LobbyCode, placement, card);
        }

        public async Task WeatherClicked()
        {
            throw new NotImplementedException();
        }


        public async Task EnemyLaneClicked(TroopPlacement placement)
        {
            throw new NotImplementedException();
        }
    }
}
