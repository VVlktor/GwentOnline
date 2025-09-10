using GwentWebAssembly.Data;

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

        public bool IsDataValid() => !string.IsNullOrEmpty(LobbyCode);

        public PlayerIdentity GetIdentity() => _identity;

        public PlayerIdentity EnemyIdentity() => _identity == PlayerIdentity.PlayerOne ? PlayerIdentity.PlayerTwo : PlayerIdentity.PlayerOne;
    }
}
