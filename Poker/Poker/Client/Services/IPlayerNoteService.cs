using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Poker.Shared;
using Poker.Shared.Models;

namespace Poker.Client.Services
{
    public interface IPlayerNoteService
    {
        Task<CreateNoteResult> Create(CreateNoteModel model);

        Task<GetNotesResult> GetList();

        Task<DeleteTableResult> Delete(string notedPlayerName);
    }
}
