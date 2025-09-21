using GwentWebAssembly.Data.Dtos;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IAnimationService
    {
        Task ProcessAnimationQueueAsync(GameStatusDto gameStatusDto);
    }
}
