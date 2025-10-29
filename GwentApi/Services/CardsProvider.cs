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

        public GwentBoardCard CreateGwentBoardCard(GwentCard gwentCard, PlayerIdentity identity)
        {
            return new()
            {
                PrimaryId = gwentCard.PrimaryId,
                CardId = gwentCard.CardId,
                Abilities = gwentCard.Abilities,
                Strength = gwentCard.Strength,
                Faction = gwentCard.Faction,
                FileName = gwentCard.FileName,
                Name = gwentCard.Name,
                Owner = identity,
                Placement = gwentCard.Placement
            };
        }

        public GwentCard GetCardByCardId(int id)
        {
            GwentCard card = Cards.First(x=>x.CardId == id);

            GwentCard newCard = new GwentCard()
            {
                Abilities = card.Abilities,
                CardId = card.CardId,
                Faction = card.Faction,
                Name = card.Name,
                Placement = card.Placement,
                PrimaryId = card.PrimaryId,
                Strength = card.Strength,
                FileName = card.FileName
            };

            return newCard;
        }

        public GwentCard GetCardByPrimaryId(int id) {
            GwentCard card = Cards.First(x => x.PrimaryId == id);

            GwentCard newCard = new GwentCard()
            {
                Abilities = card.Abilities,
                CardId = card.CardId,
                Faction = card.Faction,
                Name = card.Name,
                Placement = card.Placement,
                PrimaryId = card.PrimaryId,
                Strength = card.Strength,
                FileName = card.FileName
            };

            return newCard;
        }

        public List<GwentCard> GetCards(Func<GwentCard, bool> predicate)
        {
            return Cards.Where(predicate).Select(c => new GwentCard
            {
                Name = c.Name,
                PrimaryId = c.PrimaryId,
                CardId = c.CardId,
                Faction = c.Faction,
                Placement = c.Placement,
                Strength = c.Strength,
                Abilities = c.Abilities,
                FileName = c.FileName
            }).ToList();
        }
    }
}
