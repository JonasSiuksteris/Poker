using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared
{
    public class GamePlayer
    {
        public string Username { get; set; }

        public bool IsReady { get; set; }

        public bool IsPlaying { get; set; }

        public PlayerActionState ActionState { get; set; } = PlayerActionState.Playing;

        public int SeatNumber { get; set; }

        public int GameMoney { get; set; }

        public GamePlayer()
        {
            IsReady = false;
            IsPlaying = false;
        }


    }
}
