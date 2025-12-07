using GwentShared.Classes;

namespace GwentApi.Services.Interfaces
{
    public interface IDeckService
    {
        ResponseData VerifyDeck(PlayerDeckInfo playerInfo);
    }
}
