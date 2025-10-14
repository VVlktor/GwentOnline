using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;
using GwentWebAssembly.Services.Interfaces;
using Microsoft.JSInterop;

namespace GwentWebAssembly.Services
{
    public class AnimationService : IAnimationService
    {
        private readonly IJSRuntime _jsRuntime;
        private PlayerService _playerService;

        public AnimationService(IJSRuntime js, PlayerService playerService)
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

        public async Task ProcessReceivedAnimation(GameStatusDto gameStatusDto)
        {
            //if (gameStatusDto.Action.CardsPlayed.Count == 0) return;

            switch (gameStatusDto.Action.ActionType)
            {
                case GwentActionType.NormalCardPlayed:
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
                case GwentActionType.Pass:
                    await PlayPassAnimation(gameStatusDto);
                    break;
                case GwentActionType.EndRound:
                    await OverlayAnimation("Koniec rundy!");//moze cos wiecej, np. zmiatanie kart z planszy
                    break;
            }
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

            //dziwnie to troche wyglada, chyba obejdzie sie bez
            //if (isPlayer)
            //{
            //    await Task.Delay(800);
            //    return;
            //}

            //if (gameStatusDto.PlayerDeckCount != 0)//zamiast someOtherName dać tył karty z danej talii
            //    await _jsRuntime.InvokeVoidAsync("moveCardByElementIds", "deck-me", "hand-row", $"img/cards/someOtherName");//potencjalnie bedzie trzeba dodac jendak jakie karty zostaly dodane do eq gracza, ale wsm nie teraz, moze sie obejdzie
        }

        private async Task PlayWeatherCardAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            string startName = "deck-name-op", endName = "weather";

            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
                startName = $"card-in-hand-{boardCard.PrimaryId}";

            await _jsRuntime.InvokeVoidAsync("moveCardByElementIds", startName, endName, $"img/cards/{boardCard.FileName}");
        }

        private async Task PlayDecoyAnimation(GameStatusDto gameStatusDto)//chyba bedzie trzeba dac to wszystko na publiczne i wywolac czesc przed podmianą statusu i część po podmianie statusu
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
                startName = $"card-in-hand-{boardCard.PrimaryId}";
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
