using GwentApi.Classes;
using GwentApi.Classes.Dtos;

namespace GwentApi.Services.Interfaces
{
    public interface ICardServiceValidator
    {
        bool ValidateLaneClicked(Game game, PlayerIdentity identity, LaneClickedDto laneClickedDto);
    }
}
