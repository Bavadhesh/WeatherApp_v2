using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using weather.Models;
using Microsoft.AspNetCore.Identity;

namespace weather.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserReg : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public UserReg(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user){
                user.Password = new PasswordHasher<object>().HashPassword(null, user.Password);
                _appDbContext.User.Add(user);
                await _appDbContext.SaveChangesAsync();
                return Ok(user);
        }
    }
}