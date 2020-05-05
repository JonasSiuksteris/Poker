using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Poker.Shared;

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

        public Task AddUserToTable(int tableId, string userId);

        public Task RemoveUserFromTable(int tableId, string userId);
    }
}
