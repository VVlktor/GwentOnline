using GwentWebAssembly.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace GwentWebAssembly.Services
{
    public class GwentHubService : IGwentHubService
    {
        private HubConnection _connection;

        public GwentHubService()
        {
            _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5277/gwenthub")
            .Build();

        }
    }
}
