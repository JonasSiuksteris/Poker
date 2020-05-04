using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Poker.Client.Services;
using Poker.Shared;
using Poker.Shared.Models;

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
            await GetTablesList();
        }

        protected async void EditHandler(string message)
        {
            await GetTablesList();
            StateHasChanged();
        }

        public async void Refresh()
        {
            await GetTablesList();
            StateHasChanged();
        }

        private async Task GetTablesList()
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
