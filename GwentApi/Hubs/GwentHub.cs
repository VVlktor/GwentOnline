using GwentApi.Classes;
using GwentApi.Classes.Dtos;
using GwentApi.Extensions;
using GwentApi.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GwentApi.Hubs
{
    public class GwentHub : Hub
    {
        private IGameService _gameService;

        public GwentHub(IGameService gameService)
        {
            _gameService=gameService;
        }

        public async Task JoinBoard(string code)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, code);
        }

        public async Task LeaveBoard(string code)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, code);
        }

        public async Task LaneClicked(LaneClickedDto laneClickedDto)
        {
            GwentBoardCard boardCard = await _gameService.LaneClicked(laneClickedDto);
            if (boardCard is not null)
            {
                await _gameService.UpdateBoardState(laneClickedDto.Code);
                await _gameService.AddGwentAction(laneClickedDto, boardCard);


                GameStatusDto playerGameStatus = await _gameService.GetStatus(laneClickedDto.Code, laneClickedDto.Identity);
                PlayerIdentity enemyIdentity = laneClickedDto.Identity.GetEnemy();
                GameStatusDto enemyGameStatus = await _gameService.GetStatus(laneClickedDto.Code, enemyIdentity);

                await Clients.Caller.SendAsync("LaneMove", playerGameStatus);
                await Clients.OthersInGroup(laneClickedDto.Code).SendAsync("LaneMove", enemyGameStatus);
            }
        }
    }
}
