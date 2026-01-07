using GwentApi.Classes;
using GwentShared.Classes;

namespace GwentApi.Services.Interfaces
{
    public interface IStatusService
    {
        Task UpdateBoardState(string code);
        Task<GwentAction> AddGwentAction(PlayerIdentity identity, string code, GwentActionType actionType, List<GwentBoardCard> playedCards, List<GwentBoardCard> killedCards, List<GwentCard> drawnCards, bool leaderUsed = false);
        Task<TurnStatus> UpdateTurn(string code);
        Task EndRound(string code);
    }
}
