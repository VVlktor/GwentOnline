namespace GwentWebAssembly.Services.Interfaces
{
    public interface ILobbySetupService
    {
        Task SendLobbyReady();
        Task JoinBoardAsync();
        Task CardsSelected();
    }
}
