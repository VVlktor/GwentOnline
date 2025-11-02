using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IStatusService
    {
        Task ReceivedStatus(GameStatusDto gameStatusDto);
        Task InitializeAsync(StartStatusDto startStatus);
        void CardSelected(GwentCard card);

        GwentCard GetSelectedCard();

        event Func<Task>? OnStateChanged;

        GwentCard SelectedCard { get; set; }
    }
}
