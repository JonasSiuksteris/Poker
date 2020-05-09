using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Poker.Shared;

namespace Poker.Client.Shared
{
    public class SessionPlayerInfoBase : ComponentBase
    {
        [Parameter] public GameInformation GameInformation { get; set; }

        [Parameter] public GamePlayer GamePlayer { get; set; }

        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public AuthenticationState AuthState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            await base.OnInitializedAsync();
        }
    }
}
