using Poker.Shared.Models;
using System.Threading.Tasks;

namespace Poker.Client.Services
{
    public interface IPlayerNoteService
    {
        Task<CreateNoteResult> Create(CreateNoteModel model);

        Task<GetNotesResult> GetList();

        Task<DeleteTableResult> Delete(string notedPlayerName);
    }
}
