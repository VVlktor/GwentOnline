using GwentShared.Classes;
using GwentWebAssembly.Services.Interfaces;
using Microsoft.JSInterop;

namespace GwentWebAssembly.Services;

public class DeckBuilderService : IDeckBuilderService
{
    private ICardService _cardService;
    private ILocalStorageService _localStorageService;
    private IDeckService _deckService;

    public DeckBuilderService(IDeckService deckService, ILocalStorageService localStorageService, ICardService cardService)
    {
        _cardService = cardService;
        _deckService = deckService;
        _localStorageService = localStorageService;
    }

    public event Action? OnChange;

    public List<GwentCard> CardList { get; private set; } = [];
    public List<GwentCard> SelectedCards { get; private set; } = [];
    public List<GwentCard> LeaderCards { get; private set; } = [];

    public GwentCard SelectedLeader { get; private set; } = new() { Name = "" };
    public CardFaction SelectedFaction { get; private set; } = CardFaction.NorthernRealms;

    public string ResponseMessage { get; private set; } = "";

    private int leaderIndex = 0;
    public bool IsPrepared { get; private set; } = false;

    private int[] notImplementedLeaders = [58, 61, 60, 95, 96, 97, 144];//tymczasowe, do wywalenia

    public async Task InitializeAsync()
    {
        CardList = await _cardService.GetCardData();
        LeaderCards = CardList.Where(x => x.Placement == TroopPlacement.Leader && x.Faction == SelectedFaction).Where(x => !notImplementedLeaders.Contains(x.CardId)).ToList();
        SelectedLeader = LeaderCards.First();
    }

    public void PostInitialization()
    {
        PlayerDeckInfo? data = _localStorageService.GetItem<PlayerDeckInfo>("deck");
        if (data is null) return;
        SetFaction(data.Faction);
        SelectedCards = CardList.Where(x => data.CardsId.Contains(x.PrimaryId) && (x.Faction is CardFaction.Special or CardFaction.Neutral or CardFaction.Weather || x.Faction == data.Faction)).ToList();
        foreach (var card in SelectedCards)
            CardList.Remove(card);
        SelectedLeader = LeaderCards.FirstOrDefault(x => x.PrimaryId == data.LeaderCardId) ?? LeaderCards.First();
        OnChange?.Invoke();
    }

    public void NextLeader()
    {
        if (IsPrepared) return;
        leaderIndex++;
        if (leaderIndex >= LeaderCards.Count) leaderIndex = 0;
        SelectedLeader = LeaderCards[leaderIndex];
    }

    public void AddSelectedCard(GwentCard card)
    {
        if (IsPrepared) return;
        SelectedCards.Add(card);
        CardList.Remove(card);
    }

    public void RemoveSelectedCard(GwentCard card)
    {
        if (IsPrepared) return;
        CardList.Add(card);
        SelectedCards.Remove(card);
    }

    public async Task DeckPrepared()
    {
        if (notImplementedLeaders.Contains(SelectedLeader.CardId)) return;
        IsPrepared = true;
        PlayerDeckInfo playerInfo = new(SelectedCards.Select(x => x.PrimaryId).ToList(), SelectedLeader.PrimaryId, SelectedFaction);
        ResponseData responseData = await _deckService.VerifyAndSetDeck(playerInfo);
        ResponseMessage = responseData.Message;
        OnChange?.Invoke();
        IsPrepared = responseData.IsValid;
        if (responseData.IsValid)
            _localStorageService.SetItem("deck", playerInfo);
    }

    public void SetFaction(CardFaction faction)
    {
        if (IsPrepared) return;
        if (faction == CardFaction.Skellige) return;
        SelectedFaction = faction;
        CardList.AddRange(SelectedCards);
        SelectedCards.Clear();
        LeaderCards = CardList.Where(x => x.Placement == TroopPlacement.Leader && x.Faction == SelectedFaction).ToList();
        SelectedLeader = LeaderCards.First();
        leaderIndex = 0;
    }
}
