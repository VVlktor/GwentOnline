using GwentApi.Classes;
using GwentApi.Classes.Dtos;
using GwentApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GwentApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : Controller
    {
        private IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet("ReadyForGame/{code}/{player}")]
        public async Task<IActionResult> ReadyForGame(string code, PlayerIdentity player)
        {
            ReadyDto result = await _gameService.ReadyForGame(code, player);
            return Ok(result);
        }

        [HttpGet("PlayersReady/{code}")]
        public async Task<IActionResult> PlayersReady(string code)
        {
            ReadyDto result = await _gameService.PlayersReady(code);
            return Ok(result);
        }

        [HttpGet("StartStatus/{code}/{player}")]
        public async Task<IActionResult> StartStatus(string code, PlayerIdentity player)
        {
            StartStatusDto status = await _gameService.StartStatus(code, player);
            return Ok(status);
        }

        [HttpGet("GameStatus/{code}/{player}/{lasstActionId}")]
        public async Task<IActionResult> GameStatus(string code, PlayerIdentity player)//useless chyba...?
        {
            GameStatusDto gameStatus = await _gameService.GetStatus(code, player);
            return Ok(gameStatus);
        }

        [HttpGet("GameAction/{code}/{player}")]
        public async Task<IActionResult> GameAction(string code, PlayerIdentity player, [FromBody]GwentAction action)//nie gwentaction tylko jeszcze inny obiekt, zmienic.
        {//pewnie useless as well. Poki co jednak zostawie
            return Ok();
        }
    }
}
