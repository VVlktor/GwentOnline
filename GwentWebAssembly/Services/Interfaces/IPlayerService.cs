using GwentWebAssembly.Data;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IPlayerService
    {
        string LobbyCode { get; set; }
        void SetData(PlayerIdentity identity, string lobbyCode);
        bool IsDataValid();
        PlayerIdentity GetIdentity();
        PlayerIdentity EnemyIdentity();
    }
}
