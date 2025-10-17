using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IStatusService
    {
        Task ReceivedStatus(GameStatusDto gameStatusDto);
        Task InitializeAsync(StartStatusDto startStatus);
        //Task GwentActionHornClicked(TroopPlacement placement);
        void CardSelected(GwentCard card);
        //Task GwentActionLeaderClicked();
        //Task GwentActionLaneClicked(TroopPlacement placement);
        GwentCard GetSelectedCard();

        event Func<Task>? OnStateChanged;


        //Wiem, powinienem to zrobic powiadamiajac ui o zmianie zamiast wystawiac to jako publiczne.
        //Obiecuje ze to zrobie, ale chce juz zagrac w gwinta ze znajomymi, zalezy mi na czasie.
        PlayerIdentity Turn { get; set; }

        GwentCard SelectedCard { get; set; }

        List<GwentBoardCard> CardsOnBoard { get; set; }
        List<GwentCard> CardsInHand { get; set; }
        List<GwentCard> PlayerUsedCards { get; set; }

        int EnemyCardsCount { get; set; } 
        int EnemyDeckCount { get; set; }
        int EnemyUsedCardsCount { get; set; }
        int PlayerDeckCount { get; set; }

        GwentCard PlayerLeaderCard { get; set; }
        GwentCard EnemyLeaderCard { get; set; }

        int PlayerHp { get; set; }
        int EnemyHp { get; set; }

        int EnemyPoints { get; set; }
        int PlayerPoints { get; set; }
    }
}
