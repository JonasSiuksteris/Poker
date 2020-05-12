using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Poker.Server.Data;
using Poker.Shared;

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

        //public async Task AddUserToTable(int tableId, string userId)
        //{
        //    var newPlayer = new PlayerTable
        //    {
        //        TableId = tableId,
        //        UserId = userId
        //    };
        //    await _appDbContext.PlayerTables.AddAsync(newPlayer);
        //    await _appDbContext.SaveChangesAsync();
        //}

        //public async Task RemoveUserFromTable(int tableId, string userId)
        //{

        //    var result = await _appDbContext.PlayerTables.FirstOrDefaultAsync(e => e.UserId == userId && e.TableId == tableId);
        //    if (result == null) return;

        //    _appDbContext.PlayerTables.Remove(result);
        //    await _appDbContext.SaveChangesAsync();
        //}
    }
}
