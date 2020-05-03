using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Poker.Client.Services;
using Poker.Shared;

namespace Poker.Client.Pages
{
    public class TableListBase : ComponentBase
    {
        [Inject]
        public ITableService TableService { get; set; }

        public IEnumerable<PokerTable> PokerTables { get; set; }

        public bool ShowError { get; set; }
        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ShowError = false;

            var result = await TableService.GetList();

            if (result.Successful)
            {
                PokerTables = result.PokerTables;
            }
            else
            {
                ShowError = true;
                ErrorMessage = result.Error;
            }
        }
    }
}
