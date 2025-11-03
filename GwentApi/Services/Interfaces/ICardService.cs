using GwentApi.Classes.Dtos;
using GwentApi.Classes.GwentActionResults;

namespace GwentApi.Services.Interfaces
{
    public interface ICardService
    {
        Task<HornClickedGwentActionResult> HornClicked(HornClickedDto hornClickedDto);
        Task<LaneClickedGwentActionResult> LaneClicked(LaneClickedDto laneClickedDto);
        Task<CardClickedGwentActionResult> CardClicked(CardClickedDto cardClickedDto);
        Task<WeatherClickedGwentActionResult> WeatherClicked(WeatherClickedDto weatherClickedDto);
        Task<EnemyLaneClickedGwentActionResult> EnemyLaneClicked(EnemyLaneClickedDto enemyLaneClickedDto);
        Task<PassClickedGwentActionResult> PassClicked(PassClickedDto passClickedDto);
        Task<LeaderClickedGwentActionResult> LeaderClicked(LeaderClickedDto leaderClickedDto);
        Task<CarouselCardClickedGwentActionResult> CarouselCardClicked(CarouselCardClickedDto carouselCardClickedDto);
    }
}
