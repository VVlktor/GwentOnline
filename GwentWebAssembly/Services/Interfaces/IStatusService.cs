using GwentWebAssembly.Data.Dtos;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IStatusService
    {
        Task TestStatusMethod(GameStatusDto gameStatusDto);
        GameStatusDto GetStatusDto();
    }
}
