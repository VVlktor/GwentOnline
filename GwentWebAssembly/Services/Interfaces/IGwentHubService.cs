using GwentWebAssembly.Data;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IGwentHubService
    {
        Task SendLaneClicked(PlayerIdentity identity, string code, TroopPlacement placement, GwentCard card);
        Task JoinBoardAsync(string code);
        Task SendHornClicked(PlayerIdentity identity, string code, TroopPlacement placement, GwentCard card);
        Task SendCardClicked(PlayerIdentity identity, string code, GwentBoardCard clickedCard, GwentCard selectedCard);
    }
}
