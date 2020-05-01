using System.Threading.Tasks;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using Poker.Models.ViewModels;
using Poker.Web.Service;

namespace Poker.Web.Pages
{
    public class SignInBase : ComponentBase
    {
        [Inject]
        public IAccountService AccountService { get; set; }

        [CascadingParameter] 
        public BlazoredModalInstance BlazoredModal { get; set; }

        public RegistrationUser User { get; set; } = new RegistrationUser();

        public string ErrorMessage { get; set; } = string.Empty;

        protected async Task CreateAccount()
        {
            var result = await AccountService.RegisterAccount(User);
            if (result.IsSuccessful == true)
            {
                BlazoredModal.Close();
            }
            else
            {
                ErrorMessage = result.ErrorsList[0];
                StateHasChanged();
            }
        }
    }

}
