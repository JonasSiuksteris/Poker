using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

        [Inject] public NavigationManager NavigationManager { get; set; }

        protected async Task Delete()
        {
            var result = await TableService.Delete(Table.Id);

            if (result.Successful)
            {
                await OnChange.InvokeAsync("List was changed");
            }


        }
    }
}
