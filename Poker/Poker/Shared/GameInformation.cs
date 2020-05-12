using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared
{
    public class GameInformation
    {
        public List<GamePlayer> Players { get; set; }
        public List<Card> TableCards { get; set; }
        public List<Card> Hand { get; set; }
        public bool GameInProgress { get; set; }
        public string CurrentPlayer { get; set; }
        public int SmallBlindIndex { get; set; }
        public int BigBlindIndex { get; set; }
        public string Winner { get; set; }
        public int RaiseAmount { get; set; }
        public GameInformation()
        {
            TableCards = new List<Card>();
            Hand = new List<Card>();
            GameInProgress = false;
            RaiseAmount = 0;
        }

    }
}
