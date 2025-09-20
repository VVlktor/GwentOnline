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
        private readonly IJSRuntime _jsRuntime;
        private readonly Queue<GameStatusDto> _queue = new();
        private bool _isAnimating = false;
        public event Func<GameStatusDto, Task>? OnAnimationRequested;

        public StatusService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public void Enqueue(GameStatusDto state)
        {
            _queue.Enqueue(state);
            _ = ProcessQueueAsync();
        }

        private async Task ProcessQueueAsync()
        {
            if (_isAnimating) return;

            while (_queue.Count > 0)
            {
                _isAnimating = true;
                var gameStatusDto = _queue.Dequeue();

                if (gameStatusDto.Action.CardsPlayed.Count == 0) return;
                GwentBoardCard boardCard = gameStatusDto.Action.CardsPlayed[0];
                string laneName = $"playerLane{boardCard.Placement.ToString()}";

                await _jsRuntime.InvokeAsync<Task>("runCardAnimation", laneName);
                await _jsRuntime.InvokeVoidAsync("console.log", $"Koniec animacji {boardCard.Name}");
            }

            _isAnimating = false;
        }
    }

}
