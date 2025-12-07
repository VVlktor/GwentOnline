using GwentShared.Classes;
using GwentWebAssembly.Data;
using GwentWebAssembly.Services.Interfaces;

namespace GwentWebAssembly.Services
{
    public class PlayerService : IPlayerService
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
