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

        //[HttpPost]
        //public IEnumerable<string> GetPlayers([FromBody]int tableId)
        //{
        //    try
        //    {
        //        var test = HttpContext.User.Identity.Name;
        //        return _gameSessionRepository.GetPlayers(tableId);
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
    }
}
