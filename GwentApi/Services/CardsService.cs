using GwentApi.Classes;
using System.Text.Json;

namespace GwentApi.Services
{
    public class CardsService
    {
        public CardsService() {
            string json = File.ReadAllText("jsons/cards.json");
            Cards = JsonSerializer.Deserialize<List<GwentCard>>(json)!;
        }

        public List<GwentCard> Cards { get; set; }

        public GwentCard GetCardByPrimaryId(int id) {
            return Cards.First(x => x.PrimaryId==id);
        }
    }
}
