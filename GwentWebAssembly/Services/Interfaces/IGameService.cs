using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;
using System.Numerics;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IGameService
    {
        Task<StartStatusDto> GetStartStatus();
        Task LaneClicked(GwentLane lane, GwentCard card);
        Task LeaderClicked();
        Task CardClicked(GwentBoardCard card);
    }
}
