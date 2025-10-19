using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;
using System.Numerics;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IGameService
    {
        Task<StartStatusDto> GetStartStatus();
        Task PlayerLaneClicked(TroopPlacement placement);
        Task LeaderClicked();
        Task CardClicked(GwentBoardCard clickedCard);
        Task JoinBoardAsync();
        Task HornClicked(TroopPlacement placement);
        Task EnemyLaneClicked(TroopPlacement placement);
        Task WeatherClicked();
        Task PassClicked();
        Task CarouselCardClicked(CarouselSlot slot);
    }
}
