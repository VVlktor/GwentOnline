using GwentShared.Classes;
using GwentShared.Classes.Dtos;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IStatusService
    {
        Task ReceivedStatus(GameStatusDto gameStatusDto);
        Task InitializeAsync(StartStatusDto startStatus);
        void CardSelected(GwentCard card);

        GwentCard GetSelectedCard();

        event Action? OnStateChanged;

        GwentCard SelectedCard { get; set; }
    }
}
