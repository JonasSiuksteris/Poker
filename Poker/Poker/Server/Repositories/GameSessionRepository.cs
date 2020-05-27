using Poker.Server.Data;

namespace Poker.Server.Repositories
{
    public class GameSessionRepository : IGameSessionRepository
    {
        private readonly AppDbContext _appDbContext;

        public GameSessionRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        //public IEnumerable<string> GetPlayers(int id)
        //{
        //    return _appDbContext.PlayerTables.Where(e => e.TableId == id).Select(e => e.User.Email).AsEnumerable();
        //}
    }
}
