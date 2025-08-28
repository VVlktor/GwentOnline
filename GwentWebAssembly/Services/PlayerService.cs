using GwentWebAssembly.Data;
using GwentWebAssembly.Services.Interfaces;

namespace GwentWebAssembly.Services
{
    public class PlayerService
    {
        private PlayerIdentity _identity;
        public string LobbyCode { get; set; }

        public void SetData(PlayerIdentity identity, string lobbyCode)
        {
            _identity = identity;
            LobbyCode = lobbyCode;
        }

        public bool IsDataValid()
        {
            if(string.IsNullOrEmpty(LobbyCode)) return false;
            return true;
        }

        public PlayerIdentity WhichPlayer()
        {
            return _identity;
        }
    }
}
