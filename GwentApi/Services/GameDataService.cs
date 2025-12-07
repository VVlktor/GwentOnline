using GwentApi.Classes;
using GwentApi.Services.Interfaces;
using GwentShared.Classes;

namespace GwentApi.Services
{
    public class GameDataService : IGameDataService
    {
        public void AddAction(Game game, GwentActionType actionType, PlayerIdentity issuer, Abilities ability, List<GwentBoardCard> affectedCards, GwentBoardCard? playedCard = null)
        {
            int index = game.Actions.Count;
            GwentAction action = new()
            {
                Id = index,
                ActionType = actionType,
                Issuer = issuer,
                AbilitiyUsed = ability,
            };
            game.Actions.Add(action);
        }

        public PlayerSide GetEnemySide(Game game, PlayerIdentity identity) => identity == PlayerIdentity.PlayerOne ? game.PlayerTwo : game.PlayerOne;

        public int GetNextActionId(Game game) => game.Actions.Count == 0 ? 1 : game.Actions.Last().Id + 1;
        
        public PlayerSide GetPlayerSide(Game game, PlayerIdentity identity) => identity == PlayerIdentity.PlayerOne ? game.PlayerOne : game.PlayerTwo;

        public bool PlayersReady(Game game) => game.IsReady.playerTwo && game.IsReady.playerOne;

        public void SetPlayerSide(Game game, PlayerSide playerSide, PlayerIdentity identity)
        {
            if (identity == PlayerIdentity.PlayerOne)
                game.PlayerOne = playerSide;
            else
                game.PlayerTwo = playerSide;
        }

        public void SetReady(Game game, PlayerIdentity identity)
        {
            if (identity == PlayerIdentity.PlayerOne)
                game.IsReady.playerOne = true;
            else
                game.IsReady.playerTwo = true;
        }
    }
}
