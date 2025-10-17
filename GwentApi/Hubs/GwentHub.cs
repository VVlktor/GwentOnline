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
        private ILobbyService _lobbyService;

        public GwentHub(IGameService gameService, ICardService cardService, IStatusService statusService, ILobbyService lobbyService)
        {
            _gameService = gameService;
            _cardService = cardService;
            _statusService = statusService;
            _lobbyService = lobbyService;
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
            LaneClickedGwentActionResult actionResult = await _cardService.LaneClicked(laneClickedDto);

            if(actionResult is null) return;

            await _statusService.UpdateBoardState(laneClickedDto.Code);

            //dla mnie na przyszlosc: kiedy gracz zagra medyka, to bedzie jak normalna karta,
            //z tym ze w action bedzie HealerCardPlayed, ktora sprawi,
            //ze bedzie mogl wybrac u sibeie karte do revive. wtedy to bedzie druga akcja.
            //Spy: wystawi karte, animacja przenoszenia do eq, ale karta juz jest w api, wiec spy jest jak normalna karta
            //Scorch
            //Muster: jeszcze nie wiem. do przemyslenia

            await _statusService.AddGwentAction(laneClickedDto.Identity, laneClickedDto.Code, actionResult.ActionType, actionResult.PlayedCards, actionResult.KilledCards);

            TurnStatus turnStatus = await _statusService.UpdateTurn(laneClickedDto.Code);

            //string methodName = "LaneClickedNormalCard";

            //if (actionResult.ActionType == GwentActionType.MusterCardPlayed)
            //    methodName = "LaneClickedMusterCard";
            //else if (actionResult.ActionType == GwentActionType.MedicCardPlayed)
            //    methodName = "LaneClickedMedicCard";

            await SendStatus(laneClickedDto.Identity, laneClickedDto.Code, "ActionReceived");
        }

        public async Task HornClicked(HornClickedDto hornClickedDto)
        {
            GwentBoardCard gwentBoardCard = await _cardService.HornClicked(hornClickedDto);
            
            if (gwentBoardCard is null) return;

            await _statusService.UpdateBoardState(hornClickedDto.Code);
  //z SpawdzicCzyZmienicTure zabrac kogo jest next tura i dorzucic do AddGwentAction
            await _statusService.AddGwentAction(hornClickedDto.Identity, hornClickedDto.Code, GwentActionType.CommandersHornCardPlayed, new() { gwentBoardCard }, new());

            TurnStatus turnStatus = await _statusService.UpdateTurn(hornClickedDto.Code);

            await SendStatus(hornClickedDto.Identity, hornClickedDto.Code, "ActionReceived");
        }

        public async Task LeaderClicked(LeaderClickedDto leaderClickedDto)
        {
            await _cardService.LeaderClicked(leaderClickedDto);
        }

        public async Task CardClicked(CardClickedDto cardClickedDto)//tylko decoy
        {
            if (cardClickedDto.SelectedCard.CardId != 2) return;

            CardClickedGwentActionResult actionResult = await _cardService.CardClicked(cardClickedDto);

            if (actionResult is null) return;

            await _statusService.UpdateBoardState(cardClickedDto.Code);

            await _statusService.AddGwentAction(cardClickedDto.Identity, cardClickedDto.Code, actionResult.ActionType, new() { actionResult.PlayedCard }, new() { actionResult.SwappedCard });

            TurnStatus turnStatus = await _statusService.UpdateTurn(cardClickedDto.Code);

            await SendStatus(cardClickedDto.Identity, cardClickedDto.Code, "ActionReceived");
        }

        public async Task WeatherClicked(WeatherClickedDto weatherClickedDto)
        {
            WeatherClickedGwentActionResult actionResult = await _cardService.WeatherClicked(weatherClickedDto);

            if (actionResult is null) return;

            await _statusService.UpdateBoardState(weatherClickedDto.Code);

            await _statusService.AddGwentAction(weatherClickedDto.Identity, weatherClickedDto.Code, actionResult.ActionType, new() { actionResult.PlayedCard }, actionResult.RemovedCards);

            TurnStatus turnStatus = await _statusService.UpdateTurn(weatherClickedDto.Code);

            await SendStatus(weatherClickedDto.Identity, weatherClickedDto.Code, "ActionReceived");
        }

        public async Task EnemyLaneClicked(EnemyLaneClickedDto enemyLaneClickedDto)
        {
            EnemyLaneClickedGwentActionResult actionResult = await _cardService.EnemyLaneClicked(enemyLaneClickedDto);

            if (actionResult is null) return;

            await _statusService.UpdateBoardState(enemyLaneClickedDto.Code);

            await _statusService.AddGwentAction(enemyLaneClickedDto.Identity, enemyLaneClickedDto.Code, actionResult.ActionType, new() { actionResult.PlayedCard }, new());

            TurnStatus turnStatus = await _statusService.UpdateTurn(enemyLaneClickedDto.Code);

            await SendStatus(enemyLaneClickedDto.Identity, enemyLaneClickedDto.Code, "ActionReceived");
        }

        public async Task PassClicked(PassClickedDto passClickedDto)
        {
            PassClickedGwentActionResult actionResult = await _cardService.PassClicked(passClickedDto);

            if (actionResult is null) return;

            await _statusService.AddGwentAction(passClickedDto.Identity, passClickedDto.Code, actionResult.ActionType, new(), new());

            TurnStatus turnStatus = await _statusService.UpdateTurn(passClickedDto.Code);

            if (!turnStatus.EndRound)
            {
                await SendStatus(passClickedDto.Identity, passClickedDto.Code, "ActionReceived");
                return;
            }

            await _statusService.EndRound(passClickedDto.Code);

            await _statusService.AddGwentAction(passClickedDto.Identity, passClickedDto.Code, GwentActionType.EndRound, new(), new());

            await SendStatus(passClickedDto.Identity, passClickedDto.Code, "ActionReceived");
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
