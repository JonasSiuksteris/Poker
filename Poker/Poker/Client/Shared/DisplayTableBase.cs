using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Poker.Shared;

namespace Poker.Client.Shared
{
    public class DisplayTableBase : ComponentBase
    {
        [Parameter]
        public PokerTable Table { get; set; }
        [Parameter]

        public bool IsSelected { get; set; }

    }
}
