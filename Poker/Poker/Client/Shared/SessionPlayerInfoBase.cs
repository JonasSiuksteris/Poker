using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Poker.Client.Modals;
using Poker.Client.Services;
using Poker.Shared;
using Poker.Shared.Models;

namespace Poker.Client.Shared
{
    public class SessionPlayerInfoBase : ComponentBase
    {
        [Parameter] public GameInformation GameInformation { get; set; }

        [Parameter] public GamePlayer GamePlayer { get; set; }

        [Inject] public IPlayerNoteService PlayerNoteService { get; set; }

        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject] public IModalService ModalService { get; set; }

        public AuthenticationState AuthState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            await base.OnInitializedAsync();
        }

        protected async Task Edit()
        {
            var parameters = new ModalParameters();
            if(GameInformation.PlayersNotes.FirstOrDefault(e => e.NotedPlayerName == GamePlayer.Username) != null)
            {
                parameters.Add(nameof(EditNotes.CurrentNote),
                    GameInformation.PlayersNotes.First(e => e.NotedPlayerName == GamePlayer.Username).Description);
            }
            parameters.Add(nameof(EditNotes.NotedPlayerName), GamePlayer.Username);

            var formModal = ModalService.Show<EditNotes>("Edit Notes", parameters);
            var result = await formModal.Result;
            if (result.Cancelled) return;

            GameInformation.PlayersNotes = (await PlayerNoteService.GetList()).PlayerNotes;
            StateHasChanged();
        }
    }
}
