using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Poker.Shared;
using Poker.Shared.Models;

namespace Poker.Client.Services
{
    public interface ITableService
    {
        Task<CreateTableResult> Create(CreateTableModel model);

        Task<GetTablesResult> GetList();

        Task<DeleteTableResult> Delete(int id);

        Task<JoinTableResult> AddPlayer(int id);

        Task<JoinTableResult> RemovePlayer(int id);

    }
}
