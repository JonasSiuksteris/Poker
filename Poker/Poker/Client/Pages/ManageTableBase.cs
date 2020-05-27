using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Poker.Client.Modals;

namespace Poker.Client.Pages
{
    public class ManageTableBase : ComponentBase
    {
        [Inject] public IModalService ModalService { get; set; }

        protected TableList ChildComponent;

        protected async Task ShowNewTable()
        {
            var resultModal = ModalService.Show<NewTable>("Create New Table");
            var result = await resultModal.Result;

            if (!result.Cancelled)
            {
                ChildComponent.Refresh();
            }

        }
    }


}
