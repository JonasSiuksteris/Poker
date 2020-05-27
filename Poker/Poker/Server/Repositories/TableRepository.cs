using Microsoft.EntityFrameworkCore;
using Poker.Server.Data;
using Poker.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poker.Server.Repositories
{
    public class TableRepository : ITableRepository
    {
        private readonly AppDbContext _appDbContext;

        public TableRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IEnumerable<PokerTable>> GetTables()
        {
            return await _appDbContext.PokerTables.ToListAsync();
        }

        public async Task<PokerTable> GetTableById(int tableId)
        {
            return await _appDbContext.PokerTables.FirstOrDefaultAsync(e => e.Id == tableId);
        }

        public async Task<PokerTable> GetTableByName(string tableName)
        {
            return await _appDbContext.PokerTables.FirstOrDefaultAsync(e => e.Name == tableName);
        }

        public async Task<PokerTable> AddTable(PokerTable table)
        {
            var result = await _appDbContext.PokerTables.AddAsync(table);
            await _appDbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<PokerTable> UpdateTable(PokerTable table)
        {
            var result = await _appDbContext.PokerTables.FirstOrDefaultAsync(e => e.Id == table.Id);

            if (result == null) return null;

            result.MaxPlayers = table.MaxPlayers;
            result.Name = table.Name;

            return result;

        }

        public async Task<PokerTable> DeleteTable(int tableId)
        {
            var result = await _appDbContext.PokerTables.FirstOrDefaultAsync(e => e.Id == tableId);
            if (result == null) return null;

            _appDbContext.PokerTables.Remove(result);
            await _appDbContext.SaveChangesAsync();
            return result;
        }
    }
}
