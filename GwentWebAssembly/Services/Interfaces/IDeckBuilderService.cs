using GwentShared.Classes;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IDeckBuilderService
    {
        Task InitializeAsync();
        void PostInitialization();
        void NextLeader();
        void AddSelectedCard(GwentCard card);
        void RemoveSelectedCard(GwentCard card);
        Task DeckPrepared();
        void SetFaction(CardFaction faction);

        event Action? OnChange;

        List<GwentCard> CardList { get; }
        List<GwentCard> SelectedCards { get; }
        List<GwentCard> LeaderCards { get; }

        GwentCard SelectedLeader { get; }
        CardFaction SelectedFaction { get; }

        string ResponseMessage { get; }
        bool IsPrepared { get; }
    }
}
