using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Poker.Server.Repositories;
using Poker.Shared;
using Poker.Shared.Models;

namespace Poker.Server.Hubs
{
    public class RefreshHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITableRepository _tableRepository;

        public RefreshHub(UserManager<ApplicationUser> userManager,
            ITableRepository tableRepository)
        {
            _userManager = userManager;
            _tableRepository = tableRepository;
        }

        public async Task RefreshPage()
        {
            await Clients.All.SendAsync("ReceiveRefresh");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await DisconnectPlayer();
            await Clients.All.SendAsync("ReceiveRefresh");
            await base.OnDisconnectedAsync(exception);
        }

        private async Task DisconnectPlayer()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(Context.User);
                await _tableRepository.RemoveUserFromTable(currentUser.CurrentTableId, currentUser.Id);
                currentUser.CurrentTableId = 0;
                await _userManager.UpdateAsync(currentUser);
            }
            catch (Exception)
            {
                //Log
            }
        }
    }
}
