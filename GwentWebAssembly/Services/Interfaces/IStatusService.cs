using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IStatusService
    {
        Task ReceivedStatus(GameStatusDto state);
        Task InitializeAsync(StartStatusDto startStatus);
        //Task GwentActionHornClicked(TroopPlacement placement);
        void CardSelected(GwentCard card);
        //Task GwentActionLeaderClicked();
        //Task GwentActionLaneClicked(TroopPlacement placement);
        GwentCard GetSelectedCard();

        event Func<Task>? OnStateChanged;

        PlayerIdentity Turn { get; set; }
        List<GwentBoardCard> CardsOnBoard { get; set; }
        List<GwentCard> CardsInHand { get; set; }
    }
}
