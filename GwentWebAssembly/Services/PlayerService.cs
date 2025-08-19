using GwentWebAssembly.Data;
using GwentWebAssembly.Services.Interfaces;

namespace GwentWebAssembly.Services
{
    public class PlayerService
    {
        private PlayerIdentity _identity;
        public string _lobbyCode;

        public void SetData(PlayerIdentity identity, string lobbyCode)
        {
            _identity = identity;
            _lobbyCode = lobbyCode;
        }

        public bool IsDataValid()
        {
            if(string.IsNullOrEmpty(_lobbyCode)) return false;
            return true;
        }
    }
}
