using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Poker.Web.Pages;

namespace Poker.Web.Shared
{
    public class NavMenuBase : ComponentBase
    {
        public bool CollapseNavMenu { get; set; } = true;

        protected string NavMenuCssClass => CollapseNavMenu ? "collapse" : null;

        [Inject] public IModalService ModalService { get; set; }

        protected async Task ShowSignIn()
        {
            var resultModal = ModalService.Show<SignIn>("Sign In");
            var result = await resultModal.Result;
            base.StateHasChanged();
        }

        protected async Task ShowLogin()
        {
            var resultModal = ModalService.Show<LogIn>("Log in");
            var result = await resultModal.Result;
            StateHasChanged();
        }
        protected void ToggleNavMenu()
        {
            CollapseNavMenu = !CollapseNavMenu;
        }

    }
}
