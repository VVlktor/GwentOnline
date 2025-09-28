using GwentApi.Classes;
using GwentApi.Classes.Dtos;

namespace GwentApi.Services.Interfaces
{
    public interface ICardService
    {
        Task<GwentBoardCard> HornClicked(HornClickedDto hornClickedDto);
        Task<LaneClickedGwentActionResult> LaneClicked(LaneClickedDto laneClickedDto);
        Task<CardClickedGwentActionResult> CardClicked(CardClickedDto cardClickedDto);
        Task<WeatherClickedGwentActionResult> WeatherClicked(WeatherClickedDto weatherClickedDto);
        Task<EnemyLaneClickedGwentActionResult> EnemyLaneClicked(EnemyLaneClickedDto enemyLaneClickedDto);
    }
}
