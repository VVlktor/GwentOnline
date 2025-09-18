using GwentApi.Classes;
using GwentApi.Classes.Dtos;

namespace GwentApi.Services.Interfaces
{
    public interface ICardService
    {
        Task<GwentBoardCard> HornClicked(HornClickedDto hornClickedDto);
        Task<GwentBoardCard> LaneClicked(LaneClickedDto laneClickedDto);
    }
}
