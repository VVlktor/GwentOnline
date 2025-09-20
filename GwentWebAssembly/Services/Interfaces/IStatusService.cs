using GwentWebAssembly.Data.Dtos;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IStatusService
    {
        event Func<GameStatusDto, Task>? OnAnimationRequested;
        void Enqueue(GameStatusDto state);
    }
}
