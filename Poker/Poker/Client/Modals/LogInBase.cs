using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Poker.Client.Services;
using Poker.Shared.Models;
using System.Threading.Tasks;

namespace Poker.Client.Modals
{
    public class LogInBase : ComponentBase
    {
        public LoginModel User { get; set; } = new LoginModel();
        [Inject]
        public IAuthService AccountService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        protected async Task LogIn()
        {
            var result = await AccountService.Login(User);

            if (result.Successful)
            {

                BlazoredModal.Close(ModalResult.Ok(true));
            }
            else
            {
                ErrorMessage = result.Error;
            }
        }
    }
}
