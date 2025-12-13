using GwentApi.Classes;
using GwentApi.Classes.GwentActionResults;
using GwentApi.Extensions;
using GwentApi.Services.Interfaces;
using GwentShared.Classes;
using GwentShared.Classes.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace GwentApi.Hubs
{
    public class GwentHub : Hub
    {
        private IGameService _gameService;
        private ICardService _cardService;
        private IStatusService _statusService;
        private ILobbyService _lobbyService;

        public GwentHub(IGameService gameService, ICardService cardService, IStatusService statusService, ILobbyService lobbyService)
        {
            _gameService = gameService;
            _cardService = cardService;
            _statusService = statusService;
            _lobbyService = lobbyService;
        }

        public async Task JoinBoard(string code) => await Groups.AddToGroupAsync(Context.ConnectionId, code);
        
        public async Task LeaveBoard(string code) => await Groups.RemoveFromGroupAsync(Context.ConnectionId, code);

        public async Task LaneClicked(LaneClickedDto laneClickedDto)
        {
            LaneClickedGwentActionResult actionResult = await _cardService.LaneClicked(laneClickedDto);

            if(!actionResult.IsSuccess) return;

            await _statusService.UpdateBoardState(laneClickedDto.Code);

            await _statusService.AddGwentAction(laneClickedDto.Identity, laneClickedDto.Code, actionResult.ActionType, actionResult.PlayedCards, actionResult.KilledCards);

            await _statusService.UpdateTurn(laneClickedDto.Code);

            await SendStatus(laneClickedDto.Identity, laneClickedDto.Code, "ActionReceived");
        }

        public async Task HornClicked(HornClickedDto hornClickedDto)
        {
            HornClickedGwentActionResult actionResult = await _cardService.HornClicked(hornClickedDto);
            
            if (!actionResult.IsSuccess) return;

            await _statusService.UpdateBoardState(hornClickedDto.Code);

            await _statusService.AddGwentAction(hornClickedDto.Identity, hornClickedDto.Code, GwentActionType.CommandersHornCardPlayed, [actionResult.GwentBoardCard], []);

            await _statusService.UpdateTurn(hornClickedDto.Code);

            await SendStatus(hornClickedDto.Identity, hornClickedDto.Code, "ActionReceived");
        }

        public async Task LeaderClicked(LeaderClickedDto leaderClickedDto)
        {
            LeaderClickedGwentActionResult actionResult = await _cardService.LeaderClicked(leaderClickedDto);

            if (!actionResult.IsSuccess) return;

            await _statusService.UpdateBoardState(leaderClickedDto.Code);

            await _statusService.AddGwentAction(leaderClickedDto.Identity, leaderClickedDto.Code, actionResult.ActionType, [actionResult.PlayedCard], actionResult.RemovedCards, true);

            await _statusService.UpdateTurn(leaderClickedDto.Code);

            await SendStatus(leaderClickedDto.Identity, leaderClickedDto.Code, "ActionReceived");
        }

        public async Task CardClicked(CardClickedDto cardClickedDto)
        {
            CardClickedGwentActionResult actionResult = await _cardService.CardClicked(cardClickedDto);

            if (!actionResult.IsSuccess) return;

            await _statusService.UpdateBoardState(cardClickedDto.Code);

            await _statusService.AddGwentAction(cardClickedDto.Identity, cardClickedDto.Code, actionResult.ActionType, [actionResult.PlayedCard], [actionResult.SwappedCard]);

            await _statusService.UpdateTurn(cardClickedDto.Code);

            await SendStatus(cardClickedDto.Identity, cardClickedDto.Code, "ActionReceived");
        }

        public async Task WeatherClicked(WeatherClickedDto weatherClickedDto)
        {
            WeatherClickedGwentActionResult actionResult = await _cardService.WeatherClicked(weatherClickedDto);

            if (!actionResult.IsSuccess) return;

            await _statusService.UpdateBoardState(weatherClickedDto.Code);

            await _statusService.AddGwentAction(weatherClickedDto.Identity, weatherClickedDto.Code, actionResult.ActionType, [actionResult.PlayedCard], actionResult.RemovedCards);

            await _statusService.UpdateTurn(weatherClickedDto.Code);

            await SendStatus(weatherClickedDto.Identity, weatherClickedDto.Code, "ActionReceived");
        }

        public async Task EnemyLaneClicked(EnemyLaneClickedDto enemyLaneClickedDto)
        {
            EnemyLaneClickedGwentActionResult actionResult = await _cardService.EnemyLaneClicked(enemyLaneClickedDto);

            if (!actionResult.IsSuccess) return;

            await _statusService.UpdateBoardState(enemyLaneClickedDto.Code);

            await _statusService.AddGwentAction(enemyLaneClickedDto.Identity, enemyLaneClickedDto.Code, actionResult.ActionType, [actionResult.PlayedCard], []);

            await _statusService.UpdateTurn(enemyLaneClickedDto.Code);

            await SendStatus(enemyLaneClickedDto.Identity, enemyLaneClickedDto.Code, "ActionReceived");
        }

        public async Task PassClicked(PassClickedDto passClickedDto)
        {
            PassClickedGwentActionResult actionResult = await _cardService.PassClicked(passClickedDto);

            if (!actionResult.IsSuccess) return;

            await _statusService.AddGwentAction(passClickedDto.Identity, passClickedDto.Code, actionResult.ActionType, [], []);

            TurnStatus turnStatus = await _statusService.UpdateTurn(passClickedDto.Code);

            if (!turnStatus.EndRound)
            {
                await SendStatus(passClickedDto.Identity, passClickedDto.Code, "ActionReceived");
                return;
            }

            await _statusService.EndRound(passClickedDto.Code);

            await _statusService.AddGwentAction(passClickedDto.Identity, passClickedDto.Code, GwentActionType.EndRound, [], []);

            await SendStatus(passClickedDto.Identity, passClickedDto.Code, "ActionReceived");
        }

        public async Task CarouselCardClicked(CarouselCardClickedDto carouselCardClickedDto)
        {
            CarouselCardClickedGwentActionResult actionResult = await _cardService.CarouselCardClicked(carouselCardClickedDto);

            if (!actionResult.IsSuccess) return;

            await _statusService.UpdateBoardState(carouselCardClickedDto.Code);

            await _statusService.AddGwentAction(carouselCardClickedDto.Identity, carouselCardClickedDto.Code, actionResult.ActionType, actionResult.PlayedCards, actionResult.KilledCards);

            await _statusService.UpdateTurn(carouselCardClickedDto.Code);

            await SendStatus(carouselCardClickedDto.Identity, carouselCardClickedDto.Code, "ActionReceived");
        }

        private async Task SendStatus(PlayerIdentity identity, string code, string methodName)
        {
            GameStatusDto playerGameStatus = await _gameService.GetStatus(code, identity);
            PlayerIdentity enemyIdentity = identity.GetEnemy();
            GameStatusDto enemyGameStatus = await _gameService.GetStatus(code, enemyIdentity);

            await Clients.Caller.SendAsync(methodName, playerGameStatus);
            await Clients.OthersInGroup(code).SendAsync(methodName, enemyGameStatus);
        }

        public async Task LobbyReady(string code)
        {
            bool playersReady = await _lobbyService.PlayersReady(code);
            ReadyDto readyDto = new()
            {
                Ready = playersReady
            };
            await Clients.Group(code).SendAsync("LobbyReady", readyDto);
        }

        public async Task CardsSelected(string code)
        {
            ReadyDto readyDto = await _gameService.PlayersReady(code);
            await Clients.Group(code).SendAsync("CardsSelected", readyDto);
        }
    }
}
