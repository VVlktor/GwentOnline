using GwentShared.Classes;
using GwentWebAssembly.Services.Interfaces;
using System.Net.Http.Json;

namespace GwentWebAssembly.Services
{
    public class CardService : ICardService
    {
        private HttpClient _httpClient;

        public CardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<GwentCard>> GetCardData()
        {
            return await _httpClient.GetFromJsonAsync<List<GwentCard>>("json-data/cards.json");
        }
    }
}
