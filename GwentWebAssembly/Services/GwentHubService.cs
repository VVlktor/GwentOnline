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

            _connection.On<GameStatusDto>("LaneMove", async gameStatusDto =>
            {
                //_statusService.JakasAkcjaUpdate + TriggerAnimacji - sprawdzone, zwraca dane
                _statusService.Enqueue(gameStatusDto);
            });//potencjalnie obsolete

            _connection.On<GameStatusDto>("HornClicked", async gameStatusDto =>
            {
                _statusService.Enqueue(gameStatusDto);
            });

            _connection.On<GameStatusDto>("LaneClickedNormalCard", async gameStatusDto =>
            {
               _statusService.Enqueue(gameStatusDto);
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
    }
}
