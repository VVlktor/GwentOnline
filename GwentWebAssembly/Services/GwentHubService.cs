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
                //_statusService.JakasAkcjaUpdate + TriggerAnimacji
            });
        }


        public async Task SendLaneClicked(PlayerIdentity identity, string code, GwentLane lane, GwentCard card)
        {
            LaneClickedDto laneClickedDto = new()
            {
                Lane=lane,
                Identity=identity,
                Code=code,
                Card=card
            };
            await _connection.SendAsync("LaneClicked", laneClickedDto);
        }
    }
}
