namespace GwentApi.Classes
{
    public class Lobby
    {
        public Lobby(string lobbyCode)
        {
            LobbyCode = lobbyCode;
            PlayersCount = 1;
        }

        public string LobbyCode;
        public byte PlayersCount;
    }
}
