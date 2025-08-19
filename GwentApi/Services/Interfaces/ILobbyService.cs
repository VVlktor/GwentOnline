namespace GwentApi.Services.Interfaces
{
    public interface ILobbyService
    {
        public string CreateLobby();
        public bool JoinLobby(string lobbyCode);
    }
}
