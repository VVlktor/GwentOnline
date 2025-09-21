using GwentWebAssembly.Data.Dtos;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IStatusService
    {
        Task ReceivedStatus(GameStatusDto state);
    }
}
