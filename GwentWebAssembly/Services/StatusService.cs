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

        private GwentCard PlayerLeaderCard = new();
        private GwentCard EnemyLeaderCard = new();

        private int EnemyCartsCount = 0;//do zagrania
        private int EnemyDeckCount = 0;//pozostale/nieuzywane w talii
        private int EnemyUsedCards = 0;//zuzyte

        private int PlayerDeckCount = 0;
        private List<GwentCard> PlayerUsedCards = new();

        public PlayerIdentity Turn { get; set; } = PlayerIdentity.PlayerOne;
        private int PlayerHp = 2;
        private int EnemyHp = 2;

        private int EnemyPoints = 0;
        private int PlayerPoints = 0;

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
            EnemyCartsCount = startStatus.EnemyCartsCount;
            EnemyDeckCount = startStatus.EnemyDeckCount;

            if (OnStateChanged is not null)
                await OnStateChanged.Invoke();
            //statusService.OnAnimationRequested += RunAnimation;
            //powiadomic ze statehaschanged (chyba ze sie obejdzie)
        }

        public async Task ReceivedStatus(GameStatusDto state)
        {
            await _animationService.ProcessAnimationQueueAsync(state);
            CardsOnBoard = state.CardsOnBoard;
            CardsInHand = state.CardsInHand;
            //pozmieniac inne properites z GameStatusDto
            if (OnStateChanged is not null)
                await OnStateChanged.Invoke();
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
