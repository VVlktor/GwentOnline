using GwentShared.Classes;

namespace GwentApi.Extensions
{
    public static class ExtensionMethods
    {
        public static PlayerIdentity GetEnemy(this PlayerIdentity identity) => identity == PlayerIdentity.PlayerOne ? PlayerIdentity.PlayerTwo : PlayerIdentity.PlayerOne;
    }
}
