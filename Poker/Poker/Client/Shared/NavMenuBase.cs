using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Poker.Client.Pages;
using Poker.Client.Shared;

namespace Poker.Client.Shared
{
    public class NavMenuBase : ComponentBase
    {
        public bool CollapseNavMenu { get; set; } = true;

        protected string NavMenuCssClass => CollapseNavMenu ? "collapse" : null;

        [Inject] public IModalService ModalService { get; set; }

        [Parameter] public EventCallback<string> OnChange { get; set; }


        protected async Task ShowSignIn()
        {
            var resultModal = ModalService.Show<SignIn>("Sign In");
            var result = await resultModal.Result;

            if (!result.Cancelled)
            {
                await OnChange.InvokeAsync("SignIn");
            }
        }

        protected async void ShowLogin()
        {
            var resultModal = ModalService.Show<LogIn>("Log in");
            var result = await resultModal.Result;

            if (!result.Cancelled)
            {
                await OnChange.InvokeAsync("Login");
            }
        }

        protected async Task LogOut()
        {
            var resultModal = ModalService.Show<LogOut>("Log Out");
            var result = await resultModal.Result;

            if (!result.Cancelled)
            {
                await OnChange.InvokeAsync("Logout");
            }
        }
        protected void ToggleNavMenu()
        {
            CollapseNavMenu = !CollapseNavMenu;
        }

    }
}
