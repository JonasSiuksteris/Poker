using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared
{
    public enum PlayerActionState
    {
        Playing,
        Folded,
        Raised,
        HasToRaise,
        Left
    }
}
