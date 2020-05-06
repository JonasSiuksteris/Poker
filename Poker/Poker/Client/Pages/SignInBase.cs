using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using Poker.Client.Services;
using Poker.Shared.Models;

namespace Poker.Client.Pages
{
    public class SignInBase : ComponentBase
    {
        [Inject]
        public IAuthService AccountService { get; set; }

        [CascadingParameter] 
        public BlazoredModalInstance BlazoredModal { get; set; }

        public RegisterModel User { get; set; } = new RegisterModel();

        public bool ShowErrors { get; set; }
        public IEnumerable<string> Errors { get; set; }

        protected async Task CreateAccount()
        {
            ShowErrors = false;
            var result = await AccountService.Register(User);
            if (result.Successful)
            {
                BlazoredModal.Close();
            }
            else
            {
                Errors = result.Errors;
                ShowErrors = true;
            }
        }
    }

}
