using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;
using GwentWebAssembly.Services.Interfaces;
using Microsoft.JSInterop;
using System.Security.Cryptography.X509Certificates;
using static System.Net.Mime.MediaTypeNames;

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
            await Task.Delay(2000);
        }

        public async Task OverlayAnimation(PlayerIdentity turn)
        {
            string stringTurn = "Ruch przeciwnika!";
            if(_playerService.GetIdentity() == turn)
                stringTurn = "Twój ruch!";
            await _jsRuntime.InvokeVoidAsync("showOverlay", stringTurn);
            await Task.Delay(2000);
        }

        public async Task ProcessReceivedAnimation(GameStatusDto gameStatusDto)
        {
            if (gameStatusDto.Action.CardsPlayed.Count == 0) return;

            if(gameStatusDto.Action.ActionType == GwentActionType.NormalCardPlayed)
            {
                await PlayNormalCardAnimation(gameStatusDto);
                return;
            }

            if (gameStatusDto.Action.ActionType == GwentActionType.CommandersHornCardPlayed)
            {
                await PlayCommandersHornAnimation(gameStatusDto);
                return;
            }

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
                startName = "enemy-faction-label";
                endName = $"enemyHornLane{boardCard.Placement.ToString()}";
            }
            await _jsRuntime.InvokeVoidAsync("runCardAnimation", startName, endName);//potencjalnie w przyszlosci dodac efekt animacji-podswietlenia przy hornie
            await Task.Delay(1000);
        }

        private async Task PlayNormalCardAnimation(GameStatusDto gameStatusDto)
        {
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            string startName = "enemy-faction-label", endName = $"enemyLane{boardCard.Placement.ToString()}";
            if (gameStatusDto.Action.Issuer == _playerService.GetIdentity())
            {
                startName = $"card-in-hand-{boardCard.PrimaryId}";
                endName = $"playerLane{boardCard.Placement.ToString()}";
            }
            await _jsRuntime.InvokeVoidAsync("runCardAnimation", startName, endName);
            await Task.Delay(1000);
        }
    }
}
