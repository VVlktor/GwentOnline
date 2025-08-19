namespace GwentWebAssembly.Services.Interfaces
{
    public interface IHomePageService
    {
        public Task<string> CreateLobby();
        public Task<bool> JoinLobby(string lobbyCode);
    }
}
