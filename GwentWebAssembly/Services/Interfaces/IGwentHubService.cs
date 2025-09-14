using GwentWebAssembly.Data;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IGwentHubService
    {
        Task SendLaneClicked(PlayerIdentity identity, string code, GwentLane lane, GwentCard card);
        Task JoinBoardAsync(string code);
    }
}
