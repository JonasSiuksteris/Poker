using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared
{
    public class Pot
    {
        public HashSet<string> Players { get; set; } = new HashSet<string>();
        public int PotAmount { get; set; }
        public string Winner { get; set; }
    }
}
