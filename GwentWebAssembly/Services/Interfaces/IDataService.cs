using GwentShared.Classes;
using GwentShared.Classes.Dtos;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IDataService
    {
        void SetStartData(StartStatusDto startStatusDto);
        void UpdateData(GameStatusDto gameStatusDto);

        PlayerIdentity Turn { get; set; }

        List<GwentBoardCard> CardsOnBoard { get; set; }
        List<GwentCard> CardsInHand { get; set; }
        List<GwentCard> PlayerUsedCards { get; set; }

        int EnemyCardsCount { get; set; }
        int EnemyDeckCount { get; set; }
        int EnemyUsedCardsCount { get; set; }
        int PlayerDeckCount { get; set; }

        GwentLeaderCard PlayerLeaderCard { get; set; }
        GwentLeaderCard EnemyLeaderCard { get; set; }

        int PlayerHp { get; set; }
        int EnemyHp { get; set; }

        int EnemyPoints { get; set; }
        int PlayerPoints { get; set; }

        bool EnemyPassed { get; set; }
        bool PlayerPassed { get; set; }
    }
}
