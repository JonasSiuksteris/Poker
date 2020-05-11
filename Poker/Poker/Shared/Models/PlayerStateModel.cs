using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models
{
    public class PlayerStateModel
    {
        public List<GamePlayer> Players { get; set; }
        public List<Card> CommunityCards { get; set; }
        public List<Card> HandCards { get; set; }
        public bool GameInProgress { get; set; }
        public PlayerStateModel()
        {
            Players = new List<GamePlayer>();
            CommunityCards = new List<Card>();
            HandCards = new List<Card>();
        }
    }
}
