using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace Poker.Client.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] public ILocalStorageService LocalStorageService { get; set; }


        public int CurrentTable { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            CurrentTable = await LocalStorageService.GetItemAsync<int>("currentTable");
        }
    }
}
