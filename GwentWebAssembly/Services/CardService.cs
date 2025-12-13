using GwentShared.Classes;
using GwentShared.Services;
using GwentWebAssembly.Services.Interfaces;

namespace GwentWebAssembly.Services;

public class CardService : ICardService
{
    public async Task<List<GwentCard>> GetCardData() => CardsJsonReader.LoadGwentCards();
}
