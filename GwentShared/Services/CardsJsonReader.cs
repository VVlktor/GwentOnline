using GwentShared.Classes;
using System.Text.Json;

namespace GwentShared.Services;

public static class CardsJsonReader
{
    public static List<GwentCard> LoadGwentCards()
    {
        var assembly = typeof(CardsJsonReader).Assembly;
        using var stream = assembly.GetManifestResourceStream("GwentShared.jsons.cards.json");
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<List<GwentCard>>(json);
    }
}
