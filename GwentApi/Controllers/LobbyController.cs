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
        private IDeckService _deckService;

        public LobbyController(ILobbyService lobbyService, IDeckService deckService)
        {
            _lobbyService = lobbyService;
            _deckService = deckService;
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

        [HttpPost("VerifyDeck")]
        public IActionResult VerifyDeck([FromBody] PlayerInfo playerInfo)
        {
            var responseData = _deckService.VerifyDeck(playerInfo);
            return Ok(responseData);
        }

        [HttpPost("SetDeck/{lobbyCode}/{player}")]
        public async Task<IActionResult> SetDeck(string lobbyCode, int player, [FromBody] PlayerInfo playerInfo)
        {
            await _lobbyService.SetDeck(lobbyCode, player, playerInfo);
            return Ok();
        }
    }
}
