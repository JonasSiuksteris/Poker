using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Models.ViewModels
{
    public class RegisterResult
    {
        public List<string> ErrorsList { get; set; }
        public bool IsSuccessful { get; set; }

    }
}
