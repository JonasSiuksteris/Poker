using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

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
