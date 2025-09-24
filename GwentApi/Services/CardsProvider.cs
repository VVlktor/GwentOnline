using GwentApi.Classes;
using System.Text.Json;

namespace GwentApi.Services
{
    public class CardsProvider
    {
        public CardsProvider() {
            string json = File.ReadAllText("jsons/cards.json");
            Cards = JsonSerializer.Deserialize<List<GwentCard>>(json)!;
        }

        public List<GwentCard> Cards { get; set; }

        public GwentCard GetCardByPrimaryId(int id) {
            GwentCard card = Cards.First(x => x.PrimaryId == id);

            GwentCard newCard = new GwentCard()
            {
                Abilities = card.Abilities,
                CardId = card.CardId,
                Description = card.Description,
                Faction = card.Faction,
                Name = card.Name,
                Placement = card.Placement,
                PrimaryId = card.PrimaryId,
                Strength = card.Strength
            };

            return newCard;
        }
    }
}
