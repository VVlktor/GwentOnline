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
        private PlayerService _playerService;
        private ICarouselService _carouselService;

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

        public GwentCard SelectedCard { get; set; } = new();

        private GwentCard DummyCard { get; } = new();

        public event Func<Task>? OnStateChanged;

        public StatusService(IAnimationService animationService, PlayerService playerService, ICarouselService carouselService)
        {
            _playerService = playerService;
            _animationService = animationService;
            _carouselService = carouselService;
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

            string? message = PlayerHp > gameStatusDto.PlayerHp && EnemyHp > gameStatusDto.EnemyHp ? "Remis" :
                              PlayerHp > gameStatusDto.PlayerHp ? "Przeciwnik wygrał rundę" :
                              EnemyHp > gameStatusDto.EnemyHp ? "Wygrałeś rundę" : null;

            if(message is not null)
                await _animationService.OverlayAnimation(message);

            CardsOnBoard = gameStatusDto.CardsOnBoard;
            CardsInHand = gameStatusDto.CardsInHand;
            Turn = gameStatusDto.Turn;
            EnemyCardsCount = gameStatusDto.EnemyCardsCount;
            PlayerUsedCards = gameStatusDto.UsedCards;
            EnemyUsedCardsCount = gameStatusDto.EnemyUsedCardsCount;
            PlayerDeckCount = gameStatusDto.PlayerDeckCount;
            EnemyDeckCount = gameStatusDto.EnemyDeckCount;
            PlayerHp = gameStatusDto.PlayerHp;
            EnemyHp = gameStatusDto.EnemyHp;

            if (gameStatusDto.Action.ActionType == GwentActionType.MedicCardPlayed)
            {
                if(gameStatusDto.Action.Issuer == _playerService.GetIdentity())
                    _carouselService.ShowCarousel(PlayerUsedCards);
                if (OnStateChanged is not null)
                    await OnStateChanged.Invoke();
                return;
            }

            if (OnStateChanged is not null)
                await OnStateChanged.Invoke();

            SelectedCard = DummyCard;

            if (PlayerHp == 0 || EnemyHp == 0)
            {
                await _animationService.EndGameOverlayAnimation(EnemyHp == 0);//jak ostatnia runda bedzie remisem to obu graczom pokaze ze wygrali, malo wazne ale mozna poprawic
                return;
            }

            if (!isSameTurn || message is not null)
                await _animationService.OverlayAnimation(Turn);
        }

        public void CardSelected(GwentCard card)
        {
            SelectedCard = card;
        }
    }

}
