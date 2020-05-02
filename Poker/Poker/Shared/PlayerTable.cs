using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared
{
    public class PlayerTable
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int TableId { get; set; }
        public PokerTable Table { get; set; }
    }
}
