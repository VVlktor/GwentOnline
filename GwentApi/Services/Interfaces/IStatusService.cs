using GwentApi.Classes;

namespace GwentApi.Services.Interfaces
{
    public interface IStatusService
    {
        Task UpdateBoardState(string code);
        Task<GwentAction> AddGwentAction(PlayerIdentity identity, string code, GwentActionType actionType, List<GwentBoardCard> playedCards);
    }
}
