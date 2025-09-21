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

        public StatusService(IAnimationService animationService)
        {
            _animationService = animationService;
        }

        public async Task ReceivedStatus(GameStatusDto state)
        {
            await _animationService.ProcessAnimationQueueAsync(state);
        }

        //teraz trzeba tu przeniesc całą logike z GwentBoard.razor
    }

}
