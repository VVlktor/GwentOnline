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

        public PlayerIdentity Turn = PlayerIdentity.PlayerOne;//do zmiany w zaleznosci od talii

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

        public int GetNextActionId() => Actions.Last().Id+1;

        public bool PlayersReady() => IsReady.playerTwo && IsReady.playerOne;

        public void SetPlayerSide(PlayerSide playerSide, PlayerIdentity identity)
        {
            if (identity == PlayerIdentity.PlayerOne)
                PlayerOne = playerSide;
            else
                PlayerTwo = playerSide;
        }

        public PlayerSide GetPlayerSide(PlayerIdentity identity) => identity == PlayerIdentity.PlayerOne ? PlayerOne : PlayerTwo;
        
        public void AddAction(GwentActionType actionType, PlayerIdentity issuer, Abilities ability, List<GwentBoardCard> affectedCards, GwentBoardCard? playedCard = null)
        {
            int index = Actions.Count;
            GwentAction action = new()
            {
                Id=index,
                ActionType=actionType,
                Issuer=issuer,
                AbilitiyUsed=ability,
                CardsAffected=affectedCards,
                CardPlayed=playedCard,
            };
            Actions.Add(action);
        }
    }
}
