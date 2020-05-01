using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Poker.Models;
using Poker.Models.ViewModels;

namespace Poker.Web.Service
{
    public interface IAccountService
    {
        Task<RegisterResult> RegisterAccount(RegistrationUser user);
        Task<LoginResult> Login(LoginUser loginModel);
        Task Logout();
    }
}
