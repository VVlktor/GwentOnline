using GwentShared.Classes;
using GwentWebAssembly.Data;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface ICardService
    {
        Task<List<GwentCard>> GetCardData();
    }
}
