namespace GwentApi.Services.Interfaces
{
    public interface ILobbyService
    {
        public Task<string> CreateLobby();
        public Task<bool> JoinLobby(string lobbyCode);
    }
}
