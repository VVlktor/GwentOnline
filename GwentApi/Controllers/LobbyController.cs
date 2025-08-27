using GwentApi.Classes;
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
        public async Task<IActionResult> CreateLobby()
        {
            string lobbyCode = await _lobbyService.CreateLobby();
            return Ok(lobbyCode);
        }

        [HttpGet("JoinLobby/{lobbyCode}")]
        public async Task<IActionResult> JoinLobby(string lobbyCode)
        {
            bool joined = await _lobbyService.JoinLobby(lobbyCode);
            if(joined) 
                return Ok(lobbyCode);
            return BadRequest("Lobby is full or doesnt exist.");
        }

        [HttpPost("VerifyDeck/{lobbyCode}")]
        public async Task<IActionResult> VerifyDeck(string lobbyCode, [FromBody] PlayerInfo playerInfo)
        {
            
            return Conflict();
        }
    }
}
