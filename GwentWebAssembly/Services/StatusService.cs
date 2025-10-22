using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;
using GwentWebAssembly.Services.Interfaces;

namespace GwentWebAssembly.Services
{
    //tutaj przechowywac bede stan gry po stronie uzytkownika + wywolywac animacje
    public class StatusService : IStatusService
    {
        private IAnimationService _animationService;
        private PlayerService _playerService;
        private ICarouselService _carouselService;
        private IDataService _dataService;

        public GwentCard SelectedCard { get; set; } = new();

        private GwentCard DummyCard { get; } = new();

        public event Func<Task>? OnStateChanged;

        public StatusService(IAnimationService animationService, PlayerService playerService, ICarouselService carouselService, IDataService dataService)
        {
            _playerService = playerService;
            _animationService = animationService;
            _carouselService = carouselService;
            _dataService = dataService;
        }

        public GwentCard GetSelectedCard() => SelectedCard;

        public async Task InitializeAsync(StartStatusDto startStatus)
        {
            _dataService.SetStartData(startStatus);

            if (OnStateChanged is not null)
                await OnStateChanged.Invoke();

            await _animationService.OverlayAnimation(_dataService.Turn);
        }

        public async Task ReceivedStatus(GameStatusDto gameStatusDto)
        {
            await _animationService.ProcessReceivedAnimation(gameStatusDto);

            bool isSameTurn = _dataService.Turn == gameStatusDto.Turn;

            string? message = _dataService.PlayerHp > gameStatusDto.PlayerHp && _dataService.EnemyHp > gameStatusDto.EnemyHp ? "Remis" :
                              _dataService.PlayerHp > gameStatusDto.PlayerHp ? "Przeciwnik wygrał rundę" :
                              _dataService.EnemyHp > gameStatusDto.EnemyHp ? "Wygrałeś rundę" : null;

            if (message is not null)
                await _animationService.OverlayAnimation(message);

            _dataService.UpdateData(gameStatusDto);

            if (gameStatusDto.Action.ActionType == GwentActionType.MedicCardPlayed)
            {
                if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
                    _carouselService.ShowCarousel(_dataService.PlayerUsedCards);
                if (OnStateChanged is not null)
                    await OnStateChanged.Invoke();
                return;
            }

            if (OnStateChanged is not null)
                await OnStateChanged.Invoke();

            SelectedCard = DummyCard;

            if (_dataService.PlayerHp == 0 || _dataService.EnemyHp == 0)
            {
                await _animationService.EndGameOverlayAnimation(_dataService.EnemyHp == 0);//jak ostatnia runda bedzie remisem to obu graczom pokaze ze wygrali, malo wazne ale mozna poprawic
                return;
            }

            if (!isSameTurn || message is not null)
                await _animationService.OverlayAnimation(_dataService.Turn);
        }

        public void CardSelected(GwentCard card)
        {
            SelectedCard = card;
        }
    }

}
