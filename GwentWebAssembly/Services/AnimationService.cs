using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;
using GwentWebAssembly.Services.Interfaces;
using Microsoft.JSInterop;

namespace GwentWebAssembly.Services
{
    public class AnimationService : IAnimationService
    {
        private readonly IJSRuntime _jsRuntime;
        public event Func<GameStatusDto, Task>? OnAnimationRequested;

        public AnimationService(IJSRuntime js)
        {
            _jsRuntime = js;
        }

        public async Task ProcessAnimationQueueAsync(GameStatusDto gameStatusDto)
        {
            if (gameStatusDto.Action.CardsPlayed.Count == 0) return;
            GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
            string startName = $"card-in-hand-{boardCard.PrimaryId}";
            string endName = $"playerLane{boardCard.Placement.ToString()}";
            await _jsRuntime.InvokeVoidAsync("runCardAnimation", startName, endName);
            await Task.Delay(1000);
            if (OnAnimationRequested is not null)
            {
                await OnAnimationRequested.Invoke(gameStatusDto);
            }
        }
    }
}
