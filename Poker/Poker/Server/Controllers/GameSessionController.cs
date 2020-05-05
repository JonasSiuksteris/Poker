using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Poker.Server.Repositories;

namespace Poker.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameSessionController : ControllerBase
    {
        private readonly IGameSessionRepository _gameSessionRepository;

        public GameSessionController(IGameSessionRepository gameSessionRepository)
        {
            _gameSessionRepository = gameSessionRepository;
        }

        [HttpPost]
        public IEnumerable<string> GetTables([FromBody]int tableId)
        {
            try
            {
                return _gameSessionRepository.GetPlayers(tableId);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
