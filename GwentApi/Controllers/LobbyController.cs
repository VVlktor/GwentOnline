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
        public async Task<IActionResult> SetDeck(string lobbyCode, PlayerIdentity player, [FromBody] PlayerInfo playerInfo)
        {
            await _lobbyService.SetDeck(lobbyCode, player, playerInfo);
            return Ok();
        }

        [HttpGet("PlayersReady/{lobbyCode}")]
        public async Task<IActionResult> PlayersReady(string lobbyCode)
        {
            bool result = await _lobbyService.PlayersReady(lobbyCode);
            return Ok(result);
        }

        [HttpGet("GetPlayerInfo/{lobbyCode}/{player}")]
        public async Task<IActionResult> GetPlayerInfo(string lobbyCode, PlayerIdentity player)
        {
            PlayerInfo playerInfo = await _lobbyService.GetPlayerInfo(lobbyCode, player);
            return Ok(playerInfo);
        }

        [HttpPost("SwapCards/{lobbyCode}/{player}")]
        public async Task<IActionResult> SwapCards(string lobbyCode, PlayerIdentity identity, [FromBody]List<GwentCard> Cards)
        {
            await _lobbyService.SwapCards(lobbyCode, identity, Cards);
            return Ok();//yes I know this is bad. Its only technical demo at this point. This will be changed I promise (not only here)
        }
    }
}
