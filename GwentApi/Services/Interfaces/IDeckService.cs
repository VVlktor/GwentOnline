using GwentApi.Classes;

namespace GwentApi.Services.Interfaces
{
    public interface IDeckService
    {
        ResponseData VerifyDeck(PlayerInfo playerInfo);
    }
}
