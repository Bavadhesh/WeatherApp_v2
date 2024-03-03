using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using weather.Models;

namespace weather.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<Object> _password;
        public AuthController(AppDbContext context)
        {
            _context = context;
           
        }
        
        [HttpPost]
        [Route("login")]

        public IActionResult Login(LoginModel model){
                  
            var user = _context.User.FirstOrDefault(o=>o.Email_id == model.Username);
            if(user == null)
                return Unauthorized();
            var _password = new PasswordHasher<Object>();
            var result = _password.VerifyHashedPassword(user,user.Password,model.Password);
            if(result == PasswordVerificationResult.Success){
                var credentials = $"{model.Username}:{model.Password}";
                var credentialsBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
                var authenticationHeaderValue = $"Basic {credentialsBase64}";
                Request.Headers.Add("Authorization", authenticationHeaderValue);
                return Ok(authenticationHeaderValue);
            }
             return Unauthorized();
            
        }


    }
}