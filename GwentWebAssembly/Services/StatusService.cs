using GwentWebAssembly.Data;
using GwentWebAssembly.Data.Dtos;
using GwentWebAssembly.Services.Interfaces;

namespace GwentWebAssembly.Services
{
    //tutaj przechowywac bede stan gry po stronie uzytkownika + wywolywac animacje
    public class StatusService : IStatusService
    {
        private GameStatusDto _statusDto;

        public GameStatusDto GetStatusDto() => _statusDto;

        public async Task TestStatusMethod(GameStatusDto gameStatusDto)
        {
            _statusDto = gameStatusDto;
        }
    }
}
