using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Poker.Shared;

namespace Poker.Server.Repositories
{
    public interface IPlayerNotesRepository
    {
        public Task<IEnumerable<PlayerNote>> GetNotes(string userId);

        public Task<PlayerNote> AddNote(PlayerNote note);

        public Task<PlayerNote> DeleteNote(string userId, string notePlayerName);

        public Task<PlayerNote> GetNoteByName(string userId, string notePlayerName);

        public Task<PlayerNote> UpdateNote(PlayerNote playerNote);
    }
}
