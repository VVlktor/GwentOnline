using GwentApi.Classes;

namespace GwentApi.Services.Interfaces
{
    public interface IGameDataService
    {
        PlayerSide GetPlayerSide(Game game, PlayerIdentity identity);
        PlayerSide GetEnemySide(Game game, PlayerIdentity identity);
        void AddAction(Game game, GwentActionType actionType, PlayerIdentity issuer, Abilities ability, List<GwentBoardCard> affectedCards, GwentBoardCard? playedCard = null);
        void SetPlayerSide(Game game, PlayerSide playerSide, PlayerIdentity identity);
        void SetReady(Game game, PlayerIdentity identity);
        bool PlayersReady(Game game);
        int GetNextActionId(Game game);
    }
}
