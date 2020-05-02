using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using Poker.Client.Services;
using Poker.Shared;
using Poker.Shared.Models;

namespace Poker.Client.Modals
{
    public class NewTableBase : ComponentBase
    {
        [Inject] public ITableService TableService { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        public CreateTableModel PokerTable { get; set; } = new CreateTableModel();
        public bool ShowErrors { get; set; }
        public IEnumerable<string> Errors { get; set; }


        protected async Task CreateTable()
        {
            ShowErrors = false;
            var result = await TableService.Create(PokerTable);
            if (result.Successful)
            {
                BlazoredModal.Close();
            }
            else
            {
                Errors = result.Errors;
                ShowErrors = true;
            }
        }
    }
}
