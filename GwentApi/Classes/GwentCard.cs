namespace GwentApi.Classes
{
    public class GwentCard
    {
        public string Name { get; set; }
        public int PrimaryId { get; set; }
        public int CardId { get; set; }
        public CardFaction Faction { get; set; }
        public string Description { get; set; }
        public TroopPlacement Placement { get; set; }
        public int Strength { get; set; }
        public Abilities Abilities { get; set; }
    }
}
