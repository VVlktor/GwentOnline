using GwentWebAssembly.Services.Interfaces;

namespace GwentWebAssembly.Services
{
    public class LobbySetupService : ILobbySetupService
    {
        private IGwentHubService _gwentHubService;
        private PlayerService _playerService;

        public LobbySetupService(IGwentHubService gwentHubService, PlayerService playerService)
        {
            _gwentHubService = gwentHubService;
            _playerService = playerService;
        }

        public async Task JoinBoardAsync()
        {
            await _gwentHubService.JoinBoardAsync(_playerService.LobbyCode);
        }

        public async Task SendLobbyReady()
        {
            await _gwentHubService.SendLobbyReady(_playerService.LobbyCode);
        }

        public async Task CardsSelected()
        {
            await _gwentHubService.SendCardsSelected(_playerService.LobbyCode);
        }
    }
}
