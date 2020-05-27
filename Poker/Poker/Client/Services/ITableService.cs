using Poker.Shared;
using Poker.Shared.Models;
using System.Threading.Tasks;

namespace Poker.Client.Services
{
    public interface ITableService
    {
        Task<CreateTableResult> Create(CreateTableModel model);

        Task<GetTablesResult> GetList();

        Task<PokerTable> GetById(int id);

        Task<DeleteTableResult> Delete(int id);
    }
}
