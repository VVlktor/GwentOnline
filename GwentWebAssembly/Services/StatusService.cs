using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;
using GwentWebAssembly.Services.Interfaces;
using Microsoft.JSInterop;
using System.Threading.Channels;

namespace GwentWebAssembly.Services
{
    //tutaj przechowywac bede stan gry po stronie uzytkownika + wywolywac animacje
    public class StatusService : IStatusService
    {
        private IAnimationService _animationService;

        public List<GwentBoardCard> CardsOnBoard { get; set; } = new();
        public List<GwentCard> CardsInHand { get; set; } = new();

        public GwentCard PlayerLeaderCard { get; set; } = new();
        public GwentCard EnemyLeaderCard { get; set; } = new();

        public int EnemyCardsCount { get; set; } = 10;//do zagrania
        public int EnemyDeckCount { get; set; } = 0;//pozostale/nieuzywane w talii
        public int EnemyUsedCardsCount { get; set; } = 0;//zuzyte

        public int PlayerDeckCount { get; set; } = 0;
        public List<GwentCard> PlayerUsedCards { get; set; } = new();

        public PlayerIdentity Turn { get; set; } = PlayerIdentity.PlayerOne;
        public int PlayerHp { get; set; } = 2;
        public int EnemyHp { get; set; } = 2;

        public int EnemyPoints { get; set; } = 0;
        public int PlayerPoints { get; set; } = 0;

        private GwentCard SelectedCard { get; set; } = new();

        public event Func<Task>? OnStateChanged;

        public StatusService(IAnimationService animationService)
        {
            _animationService = animationService;
        }

        public GwentCard GetSelectedCard() => SelectedCard;

        public async Task InitializeAsync(StartStatusDto startStatus)
        {
            Turn = startStatus.Turn;
            PlayerDeckCount = startStatus.PlayerDeckCount;
            PlayerLeaderCard = startStatus.PlayerLeaderCard;
            EnemyLeaderCard = startStatus.EnemyLeaderCard;
            CardsInHand = startStatus.PlayerCards;
            EnemyDeckCount = startStatus.EnemyDeckCount;

            if (OnStateChanged is not null)
                await OnStateChanged.Invoke();

            await _animationService.OverlayAnimation(Turn);
        }

        public async Task ReceivedStatus(GameStatusDto gameStatusDto)
        {
            await _animationService.ProcessReceivedAnimation(gameStatusDto);

            bool isSameTurn = Turn == gameStatusDto.Turn;

            CardsOnBoard = gameStatusDto.CardsOnBoard;
            CardsInHand = gameStatusDto.CardsInHand;
            Turn = gameStatusDto.Turn;
            EnemyCardsCount = gameStatusDto.EnemyCardsCount;
            PlayerUsedCards = gameStatusDto.UsedCards;
            EnemyUsedCardsCount = gameStatusDto.EnemyUsedCardsCount;
            PlayerDeckCount = gameStatusDto.PlayerDeckCount;
            EnemyDeckCount = gameStatusDto.EnemyDeckCount;

            if (OnStateChanged is not null)
                await OnStateChanged.Invoke();

            if(!isSameTurn)
                await _animationService.OverlayAnimation(Turn);
        }

        //public async Task GwentActionHornClicked(TroopPlacement placement)
        //{
        //    if (SelectedCard is null) return;
        //    await _gameService.HornClicked(placement, SelectedCard);
        //}

        public void CardSelected(GwentCard card)
        {
            SelectedCard = card;
        }

        //public async Task GwentActionLeaderClicked()
        //{
        //    //await _gameService.LeaderClicked();
        //}

        //public async Task GwentActionLaneClicked(TroopPlacement placement)
        //{
        //    if (SelectedCard is null) return;

        //    await _gameService.PlayerLaneClicked(placement, SelectedCard);

        //    //LEPSZY POMYSL - nie wiem, stare, moze nie lepszy, ciezko stwierdzic z tyloma komentarzami: wysylam normalnie, zwracam, ale jak zwroce to pokaze menu z wybieraniem karty do wskszeszenia, a w hubie w serwisie sprawdzam poprostu czy ostatnia akcja to wystawienie medyka przez tego gracza
        //}
        //teraz trzeba tu przeniesc całą logike z GwentBoard.razor
    }

}
