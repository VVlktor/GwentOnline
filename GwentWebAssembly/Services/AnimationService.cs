using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;
using GwentWebAssembly.Services.Interfaces;
using Microsoft.JSInterop;

namespace GwentWebAssembly.Services
{
    public class AnimationService : IAnimationService
    {
        private readonly IJSRuntime _jsRuntime;
        private IPlayerService _playerService;

        public AnimationService(IJSRuntime js, IPlayerService playerService)
        {
            _jsRuntime = js;
            _playerService = playerService;
        }

        public async Task OverlayAnimation(string text)
        {
            await _jsRuntime.InvokeVoidAsync("showOverlay", text);
           
        }

        public async Task OverlayAnimation(PlayerIdentity turn)
        {
            string stringTurn = "Ruch przeciwnika!";
            if (_playerService.GetIdentity() == turn)
                stringTurn = "Twój ruch!";
            await _jsRuntime.InvokeVoidAsync("showOverlay", stringTurn);
            
        }

        public async Task ResizeCardContainters(int cardInHandCount, List<GwentBoardCard> cardsOnBoard) {
            await _jsRuntime.InvokeVoidAsync("resizeCardContainer", "hand-row", 11, 0.075, .00225, cardInHandCount);

            TroopPlacement[] placements = [TroopPlacement.Melee, TroopPlacement.Range, TroopPlacement.Siege];
            string[] sides = ["player", "enemy"];

            foreach (var placement in placements)
            {
                foreach(var side in sides)
                {
                    PlayerIdentity identity = side == "player" ? _playerService.GetIdentity() : _playerService.EnemyIdentity();
                    int cardCount = cardsOnBoard.Count(x => x.Placement == placement && x.Owner == identity);
                    await _jsRuntime.InvokeVoidAsync("resizeCardContainer", $"{side}Lane{placement.ToString()}", 10, 0.075, .00325, cardCount);
                }
            }
        }

        public async Task EndGameOverlayAnimation(string message)
        {
            await _jsRuntime.InvokeVoidAsync("showEndGameOverlay", message);
        }

        public async Task ProcessReceivedAnimation(GameStatusDto gameStatusDto)
        {
            if (gameStatusDto.Action.LeaderUsed)
            {
                string message = gameStatusDto.Action.Issuer == _playerService.GetIdentity() ? "Użyto umiejętności dowódcy" : "Przeciwnik używa umiejętności dowódcy";
                await OverlayAnimation(message);
            }

            switch (gameStatusDto.Action.ActionType)
            {
                case GwentActionType.NormalCardPlayed:
                case GwentActionType.MedicCardPlayed:
                    await PlayNormalCardAnimation(gameStatusDto);
                    break;
                case GwentActionType.CommandersHornCardPlayed:
                    await PlayCommandersHornAnimation(gameStatusDto);
                    break;
                case GwentActionType.DecoyCardPlayed:
                    await PlayDecoyAnimation(gameStatusDto);
                    break;
                case GwentActionType.WeatherCardPlayed:
                    await PlayWeatherCardAnimation(gameStatusDto);
                    break;
                case GwentActionType.SpyCardPlayed:
                    await PlaySpyCardAnimation(gameStatusDto);
                    break;
                case GwentActionType.ScorchCardPlayed:
                    await PlayScorchCardAnimation(gameStatusDto);
                    break;
                case GwentActionType.MusterCardPlayed:
                    await PlayMusterAnimation(gameStatusDto);
                    break;
                case GwentActionType.ScorchBoardCardPlayed:
                    await PlayScorchBoardCardAnimation(gameStatusDto);
                    break;
                case GwentActionType.Pass:
                    await PlayPassAnimation(gameStatusDto);
                    break;
                case GwentActionType.EndRound:
                    await OverlayAnimation("Koniec rundy!");//moze cos wiecej, np. zmiatanie kart z planszy
                    break;
            }
        }

        private async Task PlayMusterAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            string startName = "deck-name-op", endName = $"enemyLane{boardCard.Placement.ToString()}";
            
            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
            {
                startName = $"card-in-hand-{boardCard.PrimaryId}";
                endName = $"playerLane{boardCard.Placement.ToString()}";
                await _jsRuntime.InvokeVoidAsync("moveCardByElementIds", startName, endName, $"img/cards/{boardCard.FileName}");
                return;
            }

            foreach (var card in gameStatusDto.Action.CardsPlayed)
            {
                endName = $"enemyLane{card.Placement.ToString()}";
                await _jsRuntime.InvokeVoidAsync("moveCardByElementIds", startName, endName, $"img/cards/{card.FileName}");
            }
        }

        private async Task PlayScorchBoardCardAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            string startName = "deck-name-op", endName = $"enemyLane{boardCard.Placement.ToString()}";
            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
            {
                startName = $"card-in-hand-{boardCard.PrimaryId}";
                endName = $"playerLane{boardCard.Placement.ToString()}";
            }

            await _jsRuntime.InvokeVoidAsync("moveCardByElementIds", startName, endName, $"img/cards/{boardCard.FileName}");

            List<string> killedCardsIds = gameStatusDto.Action.CardsKilled.Select(x => $"card-on-board-{x.PrimaryId}").ToList();
            await _jsRuntime.InvokeVoidAsync("showScorchAnimation", killedCardsIds);
        }

        private async Task PlayScorchCardAnimation(GameStatusDto gameStatusDto)
        {
            if (!gameStatusDto.Action.LeaderUsed)
            {
                GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
                string startName = "deck-name-op", endName = "weather";

                if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
                    startName = $"card-in-hand-{boardCard.PrimaryId}";

                await _jsRuntime.InvokeVoidAsync("moveCardByElementIds", startName, endName, $"img/cards/{boardCard.FileName}");
            }

            List<string> killedCardsIds = gameStatusDto.Action.CardsKilled.Select(x => $"card-on-board-{x.PrimaryId}").ToList();
            await _jsRuntime.InvokeVoidAsync("showScorchAnimation", killedCardsIds);
        }

        private async Task PlayPassAnimation(GameStatusDto gameStatusDto)
        {
            string text = "Przeciwnik spasował!";
            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
                text = "Spasowałeś!";

            await OverlayAnimation(text);
        }

        private async Task PlaySpyCardAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];

            string startName = $"card-in-hand-{boardCard.PrimaryId}";
            string endName = $"enemyLane{boardCard.Placement.ToString()}";
            bool isPlayer = gameStatusDto.Action.Issuer != _playerService.GetIdentity();
            if (isPlayer){
                startName = "deck-name-op";
                endName = $"playerLane{boardCard.Placement.ToString()}";
            }

            await _jsRuntime.InvokeVoidAsync("moveCardByElementIds", startName, endName, $"img/cards/{boardCard.FileName}");

        }

        private async Task PlayWeatherCardAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            string startName = "deck-name-op", endName = "weather";

            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
                startName = $"card-in-hand-{boardCard.PrimaryId}";

            await _jsRuntime.InvokeVoidAsync("moveCardByElementIds", startName, endName, $"img/cards/{boardCard.FileName}");
        }

        private async Task PlayDecoyAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard decoyCard = gameStatusDto.Action.CardsPlayed[0];
            GwentBoardCard swappedCard = gameStatusDto.Action.CardsKilled[0];

            string startName = "", endName = $"card-on-board-{swappedCard.PrimaryId}";

            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
                startName = $"card-in-hand-{decoyCard.PrimaryId}";
            else
                startName = "deck-name-op";

            await _jsRuntime.InvokeVoidAsync("moveCardByElementIds", startName, endName, $"img/cards/{decoyCard.FileName}");

            startName = endName;
            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
                endName = "hand-row";
            else
                endName = "deck-name-op";

            await _jsRuntime.InvokeVoidAsync("moveCardByElementIds", startName, endName, $"img/cards/{swappedCard.FileName}");
        }

        private async Task PlayCommandersHornAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            string startName = "", endName = "";
            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
            {
                startName = gameStatusDto.Action.LeaderUsed ? "leader-me" : $"card-in-hand-{boardCard.PrimaryId}";
                endName = $"playerHornLane{boardCard.Placement.ToString()}";
            }
            else
            {
                startName = "deck-name-op";
                endName = $"enemyHornLane{boardCard.Placement.ToString()}";
            }

            await _jsRuntime.InvokeVoidAsync("moveCardByElementIds", startName, endName, $"img/cards/{boardCard.FileName}");//potencjalnie w przyszlosci dodac efekt animacji-podswietlenia przy hornie
        }

        private async Task PlayNormalCardAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            string startName = "deck-name-op", endName = $"enemyLane{boardCard.Placement.ToString()}";
            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
            {
                startName = $"card-in-hand-{boardCard.PrimaryId}";
                endName = $"playerLane{boardCard.Placement.ToString()}";
            }

            await _jsRuntime.InvokeVoidAsync("moveCardByElementIds", startName, endName, $"img/cards/{boardCard.FileName}");
        }
    }
}
