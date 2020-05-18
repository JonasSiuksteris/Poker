using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Poker.Shared;

namespace Poker.Server.Models
{
    public class Player
    {
        public string Name { get; set; }
        public List<Card> HandCards { get; set; } = new List<Card>();
        public HandStrength HandStrength { get; set; } = HandStrength.HighCard;
        public PlayerActionState ActionState { get; set; } = PlayerActionState.Playing;
        public int RoundBet { get; set; }
    }
}
