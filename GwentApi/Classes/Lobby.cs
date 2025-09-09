namespace GwentApi.Classes
{
    public class Lobby
    {
        public Lobby(string lobbyCode)
        {
            LobbyCode = lobbyCode;
            PlayersCount = 1;
            CurrentCardIndex = 0;
        }

        public string LobbyCode { get; set; }
        public byte PlayersCount { get; set; }
        public PlayerInfo PlayerOneInfo;
        public PlayerInfo PlayerTwoInfo;
        public int CurrentCardIndex {  get; set; }
        public bool ArePlayersReady()
        {
            return (PlayerOneInfo is not null && PlayerTwoInfo is not null);
        }
    }
}
