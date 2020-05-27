using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Poker.Server.Data;
using Poker.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Server.Repositories
{
    public class PlayerNoteRepository : IPlayerNotesRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _appDbContext;

        public PlayerNoteRepository(UserManager<ApplicationUser> userManager,
            AppDbContext appDbContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
        }
        public async Task<IEnumerable<PlayerNote>> GetNotes(string userId)
        {
            return await _appDbContext.PlayerNotes.Where(e => e.UserId == userId).ToListAsync();
        }

        public async Task<PlayerNote> GetNoteByName(string userId, string notePlayerName)
        {
            return await _appDbContext.PlayerNotes.FirstAsync(e =>
                e.UserId == userId && e.NotedPlayerName == notePlayerName);
        }


        public async Task<PlayerNote> AddNote(PlayerNote note)
        {
            var result = await _appDbContext.PlayerNotes.AddAsync(note);
            await _appDbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<PlayerNote> DeleteNote(string userId, string notePlayerName)
        {
            var result = await _appDbContext.PlayerNotes.FirstOrDefaultAsync(e => e.UserId == userId && e.NotedPlayerName == notePlayerName);
            if (result == null) return null;

            _appDbContext.PlayerNotes.Remove(result);
            await _appDbContext.SaveChangesAsync();
            return result;
        }

        public async Task<PlayerNote> UpdateNote(PlayerNote playerNote)
        {
            var result = await _appDbContext.PlayerNotes.FirstOrDefaultAsync(e => e.UserId == playerNote.UserId && e.NotedPlayerName == playerNote.NotedPlayerName);

            if (result == null) return null;

            _appDbContext.PlayerNotes.Remove(result);
            _appDbContext.SaveChanges();
            _appDbContext.PlayerNotes.Add(playerNote);
            _appDbContext.SaveChanges();

            return result;

        }
    }
}
