using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models
{
    public class GetNotesResult
    {
        public bool Successful { get; set; }

        public string Error { get; set; }
        public List<PlayerNote> PlayerNotes { get; set; }
    }
}
