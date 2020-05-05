using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Server.Repositories
{
    public interface IGameSessionRepository
    {
        public IEnumerable<string> GetPlayers(int id);
    }
}
