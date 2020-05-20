using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Poker.Shared;

namespace Poker.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CurrencyController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task Add(int amount, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            user.Currency += amount;
            await _userManager.UpdateAsync(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task Remove(int amount, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user.Currency >= amount)
            {
                user.Currency -= amount;
                await _userManager.UpdateAsync(user);
            }
            //error

        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<int> Balance()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return user.Currency;
        }

    }
}
