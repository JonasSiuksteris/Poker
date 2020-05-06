using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Poker.Client.Services;
using Poker.Shared;

namespace Poker.Client.Shared
{
    public class TableDeletionConfirmBase : ComponentBase
    {
        [Parameter] public PokerTable Table { get; set; }

        [Inject] public ITableService TableService { get; set; }

        [CascadingParameter] public BlazoredModalInstance BlazoredModal { get; set; }


        protected async Task DeleteConfirm()
        {

            var result = await TableService.Delete(Table.Id);

            if (result.Successful)
            {
                BlazoredModal.Close(ModalResult.Ok(true));
            }
        }
    }
}
