using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared
{
    public class Pot
    {
        public List<string> Winners { get; set; } = new List<string>();
        public int PotAmount { get; set; }
    }
}
