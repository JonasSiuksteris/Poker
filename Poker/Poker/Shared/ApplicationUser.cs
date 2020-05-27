using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Poker.Shared
{
    public class ApplicationUser : IdentityUser
    {
        public int Currency { get; set; }

        public ICollection<PlayerNote> PlayerNotes { get; set; }
    }
}
