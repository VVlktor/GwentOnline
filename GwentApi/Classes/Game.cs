namespace GwentApi.Classes
{
    public class Game
    {
        public Game(string code)
        {
            Code = code;
        }

        public PlayerSide PlayerOne = new();
        public PlayerSide PlayerTwo = new();

        public PlayerIdentity Turn;

        public string Code { get; set; }
        public DateTime LastMove = DateTime.Now;
        (bool playerOne, bool playerTwo) IsReady = (false, false);

        public void SetReady(PlayerIdentity identity)
        {
            if (identity == PlayerIdentity.PlayerOne)
                IsReady.playerOne = true;
            else
                IsReady.playerTwo = true;
        }

        public bool PlayersReady() => IsReady.playerTwo && IsReady.playerOne;

        public void SetPlayerSide(PlayerSide playerSide, PlayerIdentity identity)
        {
            if (identity == PlayerIdentity.PlayerOne)
                PlayerOne = playerSide;
            else
                PlayerTwo = playerSide;
        }
    }
}
