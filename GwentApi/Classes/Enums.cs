namespace GwentApi.Classes
{
    public enum PlayerIdentity
    {
        PlayerOne = 0,
        PlayerTwo = 1
    }

    public enum CardFaction
    {
        Neutral = 0,
        Special = 1,
        Weather = 2,
        NorthernRealms = 3,
        NilfgaardianEmpire = 4,
        Monsters = 5,
        Scoiatael = 6,
        Skellige = 7
    }

    public enum TroopPlacement
    {
        Melee = 1,
        Range = 2,
        Siege = 3,
        Agile = 4,
        Weather = 5,
        Special = 6,
        Leader = 7,
    }

    [Flags]
    public enum Abilities
    {
        None = 0,
        Hero = 1,
        Medic = 2,
        Morale = 4,
        Muster = 8,
        Avenger = 16,
        Bond = 32,
        Spy = 64,
        Horn = 128,
        Clear = 256,
        Frost = 512,
        Fog = 1024,
        Rain = 2048,
        Scorch = 4096,
        ScorchMelee = 8192,
        ScorchRange = 16384,
        ScorchSiege = 32768,
        Berserker = 65536,
        Madromede = 131072,
        AvengerKambi = 262144
    }

    public enum GwentActionType
    {
        None = 0,
        NormalCardPlayed = 1,
        MedicCardPlayed=2,
        SpyCardPlayed=3,
        DecoyCardPlayed=4,
        CommandersHornCardPlayed=5,
        HornCardPlayed=6,
        MusterCardPlayed=7,
        WeatherCardPlayer=8,
        LeaderCardPlayed=9,
        ScorchCardPlayed=10,
        Pass=11
    }

    public enum ClickableGwentAction
    {
        LaneClicked = 1,
        EnemyLaneClicked = 2,
        CardClicked = 3,
        WeatherLaneClicked = 4,
        LeaderClicked = 5,
    }

    public enum GwentLane
    {
        None = 0,
        Melee =1,
        Range=2,
        Siege=3
    }
}
