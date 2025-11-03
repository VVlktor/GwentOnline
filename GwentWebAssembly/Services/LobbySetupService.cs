using GwentWebAssembly.Services.Interfaces;

namespace GwentWebAssembly.Services
{
    public class LobbySetupService : ILobbySetupService
    {
        private IGwentHubService _gwentHubService;
        private IPlayerService _playerService;

        public LobbySetupService(IGwentHubService gwentHubService, IPlayerService playerService)
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
