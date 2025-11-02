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

        public List<GwentBoardCard> CardsOnBoard = new();
        public List<GwentAction> Actions = new();

        public PlayerIdentity Turn = PlayerIdentity.PlayerOne;

        public string Code { get; set; }
        public DateTime LastUpdate = DateTime.Now;
        public (bool playerOne, bool playerTwo) IsReady = (false, false);

        public (bool PlayerOne, bool PlayerTwo) HasPassed = (false, false);
    }
}
