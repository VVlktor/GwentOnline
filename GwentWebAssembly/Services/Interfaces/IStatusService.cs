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

        GwentCard SelectedCard { get; set; }
    }
}
