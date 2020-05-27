﻿using System.Collections.Generic;

namespace Poker.Shared.Models
{
    public class CreateTableResult
    {
        public bool Successful { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
