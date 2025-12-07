using GwentApi.Classes;
using GwentShared.Classes.Dtos;

namespace GwentApi.Services.Interfaces
{
    public interface ICardServiceValidator
    {
        bool ValidateLane(Game game, LaneClickedDto laneClickedDto);
        bool ValidateHorn(Game game, HornClickedDto hornClickedDto);
        bool ValidateCard(Game game, CardClickedDto cardClickedDto);
        bool ValidateWeather(Game game, WeatherClickedDto weatherClickedDto);
        bool ValidateEnemyLane(Game game, EnemyLaneClickedDto enemyLaneClickedDto);
        bool ValidatePass(Game game, PassClickedDto passClickedDto);
        bool ValidateLeader(Game game, LeaderClickedDto leaderClickedDto);
    }
}
