using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Poker.Shared
{
    public class ApplicationUser : IdentityUser
    {
        public int Currency { get; set; }

        public ICollection<PlayerNote> PlayerNotes { get; set; }
    }
}
