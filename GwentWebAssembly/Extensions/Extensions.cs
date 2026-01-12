using GwentShared.Classes;
using GwentWebAssembly.Data;

namespace GwentWebAssembly.Extensions;

public static class Extensions
{
    public static string GetAbilityName(this Abilities abilities) => 
        abilities.HasFlag(Abilities.Hero) ? $"{(abilities & ~Abilities.Hero).ToString().ToLower()}" : $"{abilities.ToString().ToLower()}";

    public static string GetPowerName(this Abilities abilities) => abilities.HasFlag(Abilities.Hero) ? "power_hero" : "power_normal";

    public static string GetAbilityBackgroundImageStyle(this Abilities abilities) =>
        abilities is Abilities.None or Abilities.Hero ? "" : $"background-image: url('img/icons/card_ability_{abilities.GetAbilityName()}.png');";//Todo: przeniesc to z extension metody gdziekolwiek indziej, zbyt duze polaczenie kodu ze stylami

    public static CardJsInfo GetData(this GwentBoardCard boardCard)
    {
        CardJsInfo jsInfo = GetData((GwentCard)boardCard);
        jsInfo.Strength = boardCard.CurrentStrength;
        return jsInfo;
    }

    public static CardJsInfo GetData(this GwentCard card)
    {
        CardJsInfo jsInfo = new();
        jsInfo.ImagePath = $"img/cards/{card.FileName}";
        jsInfo.IsHero = card.Abilities.HasFlag(Abilities.Hero);
        jsInfo.PlacementName = card.Placement.ToString().ToLower();
        jsInfo.AbilityName = card.Abilities.GetAbilityName();
        jsInfo.PrimaryId = card.PrimaryId;
        jsInfo.Strength = card.Strength;
        return jsInfo;
    }
}