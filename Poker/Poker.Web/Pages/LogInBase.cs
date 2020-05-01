using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Poker.Models;
using Poker.Models.ViewModels;
using Poker.Web.Service;

namespace Poker.Web.Pages
{
    public class LogInBase : ComponentBase
    {
        public LoginUser User { get; set; } = new LoginUser();
        [Inject]
        public IAccountService AccountService { get; set; }

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
