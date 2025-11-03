using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IAnimationService
    {
        Task ProcessReceivedAnimation(GameStatusDto gameStatusDto);
        Task OverlayAnimation(string text);
        Task OverlayAnimation(PlayerIdentity turn);
        Task EndGameOverlayAnimation(string message);
        Task ResizeCardContainters(int cardInHandCount, List<GwentBoardCard> cardsOnBoard);
    }
}
