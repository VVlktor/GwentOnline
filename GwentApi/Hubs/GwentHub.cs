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
        private ICardService _cardService;
        private IStatusService _statusService;

        public GwentHub(IGameService gameService, ICardService cardService, IStatusService statusService)
        {
            _gameService = gameService;
            _cardService = cardService;
            _statusService = statusService;
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
            //GwentBoardCard boardCard = await _gameService.LaneClicked(laneClickedDto);
            //if (boardCard is not null)
            //{
            //    await _gameService.UpdateBoardState(laneClickedDto.Code);
            //    await _gameService.AddGwentAction(laneClickedDto, boardCard);


            //    GameStatusDto playerGameStatus = await _gameService.GetStatus(laneClickedDto.Code, laneClickedDto.Identity);
            //    PlayerIdentity enemyIdentity = laneClickedDto.Identity.GetEnemy();
            //    GameStatusDto enemyGameStatus = await _gameService.GetStatus(laneClickedDto.Code, enemyIdentity);

            //    await Clients.Caller.SendAsync("LaneMove", playerGameStatus);
            //    await Clients.OthersInGroup(laneClickedDto.Code).SendAsync("LaneMove", enemyGameStatus);
            //}


            //generalnie tutaj rodzielic na _cardService.LaneClickedMuster, LaneClickedHealer etc.
            await _cardService.LaneClicked(laneClickedDto);
        }

        public async Task HornClicked(HornClickedDto hornClickedDto)
        {
            GwentBoardCard gwentBoardCard = await _cardService.HornClicked(hornClickedDto);
            
            if (gwentBoardCard is null) return;

            await _statusService.UpdateBoardState(hornClickedDto.Code);

            //await SprawdzicCzyZmienicTure()

            await _statusService.AddGwentAction(hornClickedDto.Identity, hornClickedDto.Code, GwentActionType.HornCardPlayed, new() { gwentBoardCard });

            GameStatusDto playerGameStatus = await _gameService.GetStatus(hornClickedDto.Code, hornClickedDto.Identity);
            PlayerIdentity enemyIdentity = hornClickedDto.Identity.GetEnemy();
            GameStatusDto enemyGameStatus = await _gameService.GetStatus(hornClickedDto.Code, enemyIdentity);

            await Clients.Caller.SendAsync("HornClicked", playerGameStatus);
            await Clients.OthersInGroup(hornClickedDto.Code).SendAsync("HornClicked", enemyGameStatus);
        }

        public async Task LeaderClicked()
        {

        }

        public async Task CardClicked()//tylko decoy
        {

        }
    }
}
