using GwentShared.Classes;
using GwentShared.Classes.Dtos;
using GwentWebAssembly.Services.Interfaces;

namespace GwentWebAssembly.Services
{
    public class StatusService : IStatusService
    {
        private IAnimationService _animationService;
        private IPlayerService _playerService;
        private ICarouselService _carouselService;
        private IDataService _dataService;

        public GwentCard SelectedCard { get; set; } = new();

        private GwentCard DummyCard { get; } = new();

        public event Action? OnStateChanged;

        public StatusService(IAnimationService animationService, IPlayerService playerService, ICarouselService carouselService, IDataService dataService)
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
                OnStateChanged.Invoke();

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
                if (gameStatusDto.Action.Issuer == _playerService.GetIdentity() && gameStatusDto.UsedCards.Count != 0)
                    _carouselService.ShowCarousel(_dataService.PlayerUsedCards.Where(x => !x.Abilities.HasFlag(Abilities.Hero) && x.Placement != TroopPlacement.Weather && x.Placement != TroopPlacement.Special && x.CardId != 2 && x.CardId != 6).ToList());
                if (OnStateChanged is not null)
                    OnStateChanged.Invoke();
                return;
            }

            if (OnStateChanged is not null)
                OnStateChanged.Invoke();

            SelectedCard = DummyCard;

            if (_dataService.PlayerHp == 0 || _dataService.EnemyHp == 0)
            {
                string endGameMessage = (_dataService.PlayerHp, _dataService.EnemyHp) switch
                {
                    (0, 0) => "Gra zakończona remisem",
                    (_, 0) => "Wygrywasz grę",
                    _ => "Przegrywasz grę"
                };

                await _animationService.EndGameOverlayAnimation(endGameMessage);
                return;
            }

            await _animationService.ResizeCardContainters(_dataService.CardsInHand.Count, _dataService.CardsOnBoard);

            if (!isSameTurn || message is not null)
                await _animationService.OverlayAnimation(_dataService.Turn);
        }

        public void CardSelected(GwentCard card) => SelectedCard = card;
    }
}
