using GwentWebAssembly.Services.Interfaces;

namespace GwentWebAssembly.Services
{
    public class LobbyInitializationService : ILobbyInitializationService
    {
        private HttpClient _httpClient;

        public LobbyInitializationService(HttpClient httpClient)
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
            return response.IsSuccessStatusCode;
        }
    }
}
