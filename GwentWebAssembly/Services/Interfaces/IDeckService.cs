using GwentWebAssembly.Data;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IDeckService
    {
        Task<ResponseData> VerifyDeck(PlayerInfo playerInfo);
    }
}
