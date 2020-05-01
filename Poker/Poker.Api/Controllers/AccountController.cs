using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Poker.Models;
using Poker.Models.ViewModels;

namespace Poker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<RegisterResult> Register(RegistrationUser model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var userExists = await _userManager.FindByEmailAsync(model.Email);

            if (userExists != null)
            {
                return new RegisterResult()
                {
                    ErrorsList = new List<string> {"User with this email already exists"},
                    IsSuccessful = false
                };
            }

            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return new RegisterResult{IsSuccessful = true};
                }

                var errors = result.Errors.Select(error => error.Description).ToList();

                return new RegisterResult()
                {
                    ErrorsList = errors,
                    IsSuccessful = false
                };
            }
            catch (Exception)
            {
                return new RegisterResult()
                {
                    ErrorsList = new List<string> { "Database error, try again later" },
                    IsSuccessful = false
                };
            }

        }

    }
}
