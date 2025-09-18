using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;
using System.Numerics;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IGameService
    {
        Task<StartStatusDto> GetStartStatus();
        Task PlayerLaneClicked(GwentLane lane, GwentCard card);
        Task LeaderClicked();
        Task CardClicked(GwentBoardCard clickedCard, GwentCard card);
        Task JoinBoardAsync();
        Task HornClicked(TroopPlacement placement, GwentCard card);
    }
}
