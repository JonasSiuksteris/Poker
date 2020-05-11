using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Poker.Client.Modals;
using Poker.Client.Pages;
using Poker.Client.Services;
using Poker.Shared;

namespace Poker.Client.Shared
{
    public class DisplayTableBase : ComponentBase
    {
        [Parameter]
        public EventCallback<string> OnChange { get; set; }

        [Parameter]
        public PokerTable Table { get; set; }

        [Inject] public ITableService TableService { get; set; }

        [Inject] public ILocalStorageService LocalStorageService { get; set; }

        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public IModalService ModalService { get; set; }

        protected async Task DeleteConfirm()
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(Table), Table);

            var resultModal = ModalService.Show<TableDeletionConfirm>("Confirm", parameters);
            var result = await resultModal.Result;

            if (!result.Cancelled)
            {
                await OnChange.InvokeAsync("List was changed");
            }
        }

        protected async Task JoinTable()
        {
            //await TableService.AddPlayer(Table.Id);

            await LocalStorageService.SetItemAsync("currentTable", Table.Id);

            NavigationManager.NavigateTo("/Game");
            
        }
    }
}
