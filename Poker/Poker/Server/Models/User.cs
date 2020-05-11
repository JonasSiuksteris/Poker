using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Server.Models
{
    public class User
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public int TableId { get; set; }
        public bool IsReady { get; set; } = false;
        public bool InGame { get; set; }
        public int SeatNumber { get; set; }

    }
}
