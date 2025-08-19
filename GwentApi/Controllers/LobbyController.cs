using GwentApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GwentApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LobbyController : Controller
    {
        private ILobbyService _lobbyService;

        public LobbyController(ILobbyService lobbyService)
        {
            _lobbyService = lobbyService;
        }

        [HttpGet("CreateLobby")]
        public IActionResult CreateLobby()
        {
            string lobbyCode = _lobbyService.CreateLobby();
            return Ok(lobbyCode);
        }

        [HttpGet("JoinLobby/{lobbyCode}")]
        public IActionResult JoinLobby(string lobbyCode)
        {
            bool joined = _lobbyService.JoinLobby(lobbyCode);
            if(joined) 
                return Ok(lobbyCode);
            return BadRequest("Lobby is full or doesnt exist.");
        }
    }
}
