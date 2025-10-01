using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;
using GwentWebAssembly.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace GwentWebAssembly.Services
{
    public class GwentHubService : IGwentHubService
    {
        private HubConnection _connection;
        private IStatusService _statusService;

        public GwentHubService(IStatusService statusService)
        {
            _statusService = statusService;

            _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5277/gwenthub")
            .Build();

            _connection.On<GameStatusDto>("HornClicked", async gameStatusDto =>
            {
                await _statusService.ReceivedStatus(gameStatusDto);
            });

            _connection.On<GameStatusDto>("LaneClickedNormalCard", async gameStatusDto =>
            {
               await _statusService.ReceivedStatus(gameStatusDto);
            });
            //LaneClickedMusterCard, medic etc.

            _connection.On<GameStatusDto>("CardClicked", async gameStatusDto =>
            {
                await _statusService.ReceivedStatus(gameStatusDto);
            });

            _connection.On<GameStatusDto>("WeatherClicked", async gameStatusDto =>
            {
                await _statusService.ReceivedStatus(gameStatusDto);
            });

            _connection.On<GameStatusDto>("EnemyLaneClicked", async gameStatusDto =>
            {
                await _statusService.ReceivedStatus(gameStatusDto);
            });//skoro to wszystko jest to samo, moze wsadzic to do jednego syfu i fajrant?

            _connection.On<GameStatusDto>("PassClicked", async gameStatusDto =>
            {
                await _statusService.ReceivedStatus(gameStatusDto);
            });
        }

        public async Task JoinBoardAsync(string code)
        {
            await _connection.StartAsync();
            await _connection.SendAsync("JoinBoard", code);
        }

        public async Task SendLaneClicked(PlayerIdentity identity, string code, TroopPlacement placement, GwentCard card)
        {
            LaneClickedDto laneClickedDto = new()
            {
                Placement = placement,
                Identity=identity,
                Code=code,
                Card=card
            };
            await _connection.SendAsync("LaneClicked", laneClickedDto);
        }

        public async Task SendHornClicked(PlayerIdentity identity, string code, TroopPlacement placement, GwentCard card)
        {
            HornClickedDto hornClickedDto = new()
            {
                Placement=placement,
                Identity=identity,
                Code=code,
                Card=card
            };

            await _connection.SendAsync("HornClicked", hornClickedDto);
        }

        public async Task SendCardClicked(PlayerIdentity identity, string code, GwentBoardCard clickedCard, GwentCard selectedCard)
        {
            CardClickedDto cardClickedDto = new()
            {
                ClickedCard=clickedCard,
                SelectedCard=selectedCard,
                Identity=identity,
                Code= code
            };

            await _connection.SendAsync("CardClicked", cardClickedDto);
        }

        public async Task SendWeatherClicked(PlayerIdentity identity, string code, GwentCard selectedCard)
        {
            WeatherClickedDto weatherClickedDto = new()
            {
                Identity=identity,
                Code=code,
                Card=selectedCard
            };

            await _connection.SendAsync("WeatherClicked", weatherClickedDto);
        }

        public async Task SendEnemyLaneClicked(PlayerIdentity identity, string code, TroopPlacement placement, GwentCard card)
        {
            EnemyLaneClickedDto enemyLaneClickedDto = new()
            {
                Placement=placement,
                Identity=identity,
                Code=code,
                Card=card
            };

            await _connection.SendAsync("EnemyLaneClicked", enemyLaneClickedDto);
        }

        public async Task SendPassClicked(PlayerIdentity identity, string code)
        {
            PassClickedDto passClickedDto = new()
            {
                Identity=identity,
                Code=code
            };

            await _connection.SendAsync("PassClicked", passClickedDto);
        }
    }
}
