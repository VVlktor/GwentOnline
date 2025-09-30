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

            string methodName = "LaneClickedNormalCard";

            if (actionResult.ActionType == GwentActionType.MusterCardPlayed)
                methodName = "LaneClickedMusterCard";
            else if (actionResult.ActionType == GwentActionType.MedicCardPlayed)
                methodName = "LaneClickedMedicCard";


            GameStatusDto playerGameStatus = await _gameService.GetStatus(laneClickedDto.Code, laneClickedDto.Identity);
            PlayerIdentity enemyIdentity = laneClickedDto.Identity.GetEnemy();
            GameStatusDto enemyGameStatus = await _gameService.GetStatus(laneClickedDto.Code, enemyIdentity);

            await Clients.Caller.SendAsync(methodName, playerGameStatus);
            await Clients.OthersInGroup(laneClickedDto.Code).SendAsync(methodName, enemyGameStatus);
        }

        public async Task HornClicked(HornClickedDto hornClickedDto)
        {
            GwentBoardCard gwentBoardCard = await _cardService.HornClicked(hornClickedDto);
            
            if (gwentBoardCard is null) return;

            await _statusService.UpdateBoardState(hornClickedDto.Code);

            //await SprawdzicCzyZmienicTure()

            //z SpawdzicCzyZmienicTure zabrac kogo jest next tura i dorzucic do AddGwentAction
            await _statusService.AddGwentAction(hornClickedDto.Identity, hornClickedDto.Code, GwentActionType.CommandersHornCardPlayed, new() { gwentBoardCard }, new());

            GameStatusDto playerGameStatus = await _gameService.GetStatus(hornClickedDto.Code, hornClickedDto.Identity);
            PlayerIdentity enemyIdentity = hornClickedDto.Identity.GetEnemy();
            GameStatusDto enemyGameStatus = await _gameService.GetStatus(hornClickedDto.Code, enemyIdentity);

            await Clients.Caller.SendAsync("HornClicked", playerGameStatus);
            await Clients.OthersInGroup(hornClickedDto.Code).SendAsync("HornClicked", enemyGameStatus);
        }

        public async Task LeaderClicked()
        {

        }

        public async Task CardClicked(CardClickedDto cardClickedDto)//tylko decoy
        {
            if (cardClickedDto.SelectedCard.CardId != 2) return;

            CardClickedGwentActionResult actionResult = await _cardService.CardClicked(cardClickedDto);

            if (actionResult is null) return;

            await _statusService.UpdateBoardState(cardClickedDto.Code);

            //sprawdzic czy zmienic ture

            await _statusService.AddGwentAction(cardClickedDto.Identity, cardClickedDto.Code, actionResult.ActionType, new() { actionResult.PlayedCard }, new() { actionResult.SwappedCard });

            GameStatusDto playerGameStatus = await _gameService.GetStatus(cardClickedDto.Code, cardClickedDto.Identity);
            PlayerIdentity enemyIdentity = cardClickedDto.Identity.GetEnemy();
            GameStatusDto enemyGameStatus = await _gameService.GetStatus(cardClickedDto.Code, enemyIdentity);

            await Clients.Caller.SendAsync("CardClicked", playerGameStatus);
            await Clients.OthersInGroup(cardClickedDto.Code).SendAsync("CardClicked", enemyGameStatus);
        }

        public async Task WeatherClicked(WeatherClickedDto weatherClickedDto)
        {
            WeatherClickedGwentActionResult actionResult = await _cardService.WeatherClicked(weatherClickedDto);

            if (actionResult is null) return;

            await _statusService.UpdateBoardState(weatherClickedDto.Code);

            //sprawdzic czy zmienic ture

            await _statusService.AddGwentAction(weatherClickedDto.Identity, weatherClickedDto.Code, actionResult.ActionType, new() { actionResult.PlayedCard }, actionResult.RemovedCards);

            GameStatusDto playerGameStatus = await _gameService.GetStatus(weatherClickedDto.Code, weatherClickedDto.Identity);
            PlayerIdentity enemyIdentity = weatherClickedDto.Identity.GetEnemy();
            GameStatusDto enemyGameStatus = await _gameService.GetStatus(weatherClickedDto.Code, enemyIdentity);

            await Clients.Caller.SendAsync("WeatherClicked", playerGameStatus);
            await Clients.OthersInGroup(weatherClickedDto.Code).SendAsync("WeatherClicked", enemyGameStatus);
        }

        public async Task EnemyLaneClicked(EnemyLaneClickedDto enemyLaneClickedDto)
        {
            EnemyLaneClickedGwentActionResult actionResult = await _cardService.EnemyLaneClicked(enemyLaneClickedDto);

            if (actionResult is null) return;

            await _statusService.UpdateBoardState(enemyLaneClickedDto.Code);

            //sprawdzic czy zmienic ture

            await _statusService.AddGwentAction(enemyLaneClickedDto.Identity, enemyLaneClickedDto.Code, actionResult.ActionType, new() { actionResult.PlayedCard }, new());

            GameStatusDto playerGameStatus = await _gameService.GetStatus(enemyLaneClickedDto.Code, enemyLaneClickedDto.Identity);
            PlayerIdentity enemyIdentity = enemyLaneClickedDto.Identity.GetEnemy();
            GameStatusDto enemyGameStatus = await _gameService.GetStatus(enemyLaneClickedDto.Code, enemyIdentity);

            await Clients.Caller.SendAsync("EnemyLaneClicked", playerGameStatus);
            await Clients.OthersInGroup(enemyLaneClickedDto.Code).SendAsync("EnemyLaneClicked", enemyGameStatus);
        }
    }
}
