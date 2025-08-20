using GwentWebAssembly.Data;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GwentWebAssembly.Services
{
    public class CardService
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
