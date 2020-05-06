using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Poker.Shared.Models
{
    public class CreateTableModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(2, 8, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int MaxPlayers { get; set; }
    }
}
