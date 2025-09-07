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
            bool result = await _gameService.ReadyForGame(code, player);//Todo: zmienic na dto
            return Ok(result);
        }

        [HttpGet("PlayersReady/{code}")]
        public async Task<IActionResult> PlayersReady(string code)
        {
            bool result = await _gameService.PlayersReady(code);
            return Ok(result);
        }

        [HttpGet("GameStatus/{code}/{player}")]
        public async Task<IActionResult> GameStatus(string code, PlayerIdentity player)//moze dodac int lastMoveId?
        {
            GameStatusDto gameStatus = await _gameService.GetStatus(code, player);
            return Ok();
        }
    }
}
