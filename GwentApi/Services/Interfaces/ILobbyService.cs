using GwentApi.Classes;

namespace GwentApi.Services.Interfaces
{
    public interface ILobbyService
    {
        public Task<string> CreateLobby();
        public Task<bool> JoinLobby(string lobbyCode);
        Task SetDeck(string lobbyCode, int player, PlayerInfo playerInfo);
    }
}
