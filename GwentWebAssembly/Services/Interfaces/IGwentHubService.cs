using GwentWebAssembly.Data;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IGwentHubService
    {
        Task SendLaneClicked(PlayerIdentity identity, string code, TroopPlacement placement, GwentCard card);
        Task JoinBoardAsync(string code);
        Task SendHornClicked(PlayerIdentity identity, string code, TroopPlacement placement, GwentCard card);
        Task SendCardClicked(PlayerIdentity identity, string code, GwentBoardCard clickedCard, GwentCard selectedCard);
        Task SendWeatherClicked(PlayerIdentity identity, string code, GwentCard selectedCard);
        Task SendEnemyLaneClicked(PlayerIdentity identity, string code, TroopPlacement placement, GwentCard card);
        Task SendPassClicked(PlayerIdentity identity, string code);
        Task SendLeaderClicked(PlayerIdentity identity, string code);
    }
}
