using GwentWebAssembly.Data.Dtos;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IGameService
    {
        Task<StartStatusDto> GetStartStatus();
    }
}
