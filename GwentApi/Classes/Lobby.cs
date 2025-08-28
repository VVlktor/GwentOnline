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
        public PlayerInfo PlayerOneInfo;
        public PlayerInfo PlayerTwoInfo;

        public bool ArePlayersReady()
        {
            return (PlayerOneInfo is not null && PlayerTwoInfo is not null);
        }
    }
}
