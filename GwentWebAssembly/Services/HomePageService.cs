using GwentWebAssembly.Services.Interfaces;

namespace GwentWebAssembly.Services
{
    public class HomePageService : IHomePageService
    {
        private HttpClient _httpClient;

        public HomePageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CreateLobby()
        {
            var response = await _httpClient.GetAsync("http://localhost:5277/lobby/CreateLobby");
            if (response.IsSuccessStatusCode)
            {
                string lobbyCode = await response.Content.ReadAsStringAsync();
                return lobbyCode;
            }
            return "";
        }

        public async Task<bool> JoinLobby(string lobbyCode)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5277/lobby/JoinLobby/{lobbyCode}");
            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }
    }
}
