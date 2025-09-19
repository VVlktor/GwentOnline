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
        private CardsProvider _cardsService;

        public LobbyController(ILobbyService lobbyService, IDeckService deckService, CardsProvider cardsService)
        {
            _lobbyService = lobbyService;
            _deckService = deckService;
            _cardsService = cardsService;
        }

        [HttpGet("Cards")]
        public IActionResult GetCards()
        {
            var list = _cardsService.Cards.Where(c => HasMultipleFlags(c.Abilities));
            return Ok(list.ToList());
        }

        bool HasMultipleFlags(Abilities value)
        {
            int v = (int)value;
            return v != 0 && (v & (v - 1)) != 0;
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
        public async Task<IActionResult> VerifyAndSetDeck(string lobbyCode, PlayerIdentity player, [FromBody] PlayerInfo playerInfo)
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

        [HttpPost("ReadyForGame/{lobbyCode}/{identity}")]
        public async Task<IActionResult> ReadyForGame(string lobbyCode, PlayerIdentity identity)
        {
            return Ok();//todo: kontynuowac CO TO KURWA ZNACZY KONTYNUOWAC, CO MAM TU ZROBIC?!?!?!
        }
    }
}
