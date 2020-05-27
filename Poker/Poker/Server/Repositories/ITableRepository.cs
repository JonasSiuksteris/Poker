using Poker.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poker.Server.Repositories
{
    public interface ITableRepository
    {
        public Task<IEnumerable<PokerTable>> GetTables();

        public Task<PokerTable> GetTableById(int tableId);
        public Task<PokerTable> GetTableByName(string tableName);

        public Task<PokerTable> AddTable(PokerTable table);

        public Task<PokerTable> UpdateTable(PokerTable table);

        public Task<PokerTable> DeleteTable(int tableId);
    }
}
