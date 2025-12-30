namespace GwentWebAssembly.Services.Interfaces
{
    public interface ILobbyInitializationService
    {
        public Task<string> CreateLobby();
        public Task<bool> JoinLobby(string lobbyCode);
    }
}
