using System.Collections.Generic;

namespace Poker.Shared
{
    public class Pot
    {
        public HashSet<string> Players { get; set; } = new HashSet<string>();
        public int PotAmount { get; set; }
        public string Winner { get; set; }
    }
}
