using GwentApi.Classes;
using GwentApi.Classes.Dtos;
using GwentApi.Repository.Interfaces;
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
    }
}
