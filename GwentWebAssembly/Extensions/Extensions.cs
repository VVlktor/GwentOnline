using GwentShared.Classes;

namespace GwentWebAssembly.Extensions;

public static class Extensions
{
    public static string GetAbilityName(this Abilities abilities) => 
        abilities.HasFlag(Abilities.Hero) ? $"{(abilities & ~Abilities.Hero).ToString().ToLower()}" : $"{abilities.ToString().ToLower()}";

    public static string GetPowerName(this Abilities abilities) => abilities.HasFlag(Abilities.Hero) ? "power_hero" : "power_normal";

    public static string GetAbilityBackgroundImageStyle(this Abilities abilities) =>
        abilities is Abilities.None or Abilities.Hero ? "" : $"background-image: url('img/icons/card_ability_{abilities.GetAbilityName()}.png');";//Todo: przeniesc to z extension metody gdziekolwiek indziej, zbyt duze polaczenie kodu ze stylami
}