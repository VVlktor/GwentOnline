using GwentApi.Classes;
using GwentApi.Services;
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
            if (joined)
                return Ok(lobbyCode);
            return BadRequest("Lobby is full or doesnt exist.");
        }

        [HttpPost("VerifyAndSetDeck/{lobbyCode}/{player}")]
        public async Task<IActionResult> VerifyAndSetDeck(string lobbyCode, PlayerIdentity player, [FromBody] PlayerDeckInfo playerInfo)
        {
            var responseData = _deckService.VerifyDeck(playerInfo);
            if (!responseData.IsValid) return Ok(responseData);
            await _lobbyService.SetDeck(lobbyCode, player, playerInfo);
            return Ok(responseData);
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

        [HttpPost("SwapCard/{lobbyCode}/{identity}")]
        public async Task<IActionResult> SwapCard(string lobbyCode, PlayerIdentity identity, [FromBody] int id)
        {
            PlayerInfo playerInfo = await _lobbyService.SwapCard(lobbyCode, identity, id);
            return Ok(playerInfo);
        }
    }
}
