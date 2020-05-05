using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Poker.Server.Hubs
{
    public class RefreshHub : Hub
    {
        public async Task RefreshPage()
        {
            await Clients.All.SendAsync("ReceiveRefresh");
        }
    }
}
