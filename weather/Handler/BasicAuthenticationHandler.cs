using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using weather.Models;

namespace weather.Handler
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly AppDbContext _dbcontext;
        
        
        public BasicAuthenticationHandler(AppDbContext dbcontext,IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _dbcontext = dbcontext;
          
        }

        // protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        // {

        //     // if(!Request.Headers.ContainsKey("Authorization")){
        //     //     return AuthenticateResult.Fail("Failed");
        //     // }

        //     var _headerkey = Request.Headers["Authorization"].ToString();
        //     var bytes = Convert.FromBase64String(_headerkey.Replace("Basic ","",StringComparison.OrdinalIgnoreCase));
        //     string credentials = Encoding.UTF8.GetString(bytes);
        //     return AuthenticateResult.Fail(credentials);
        //     if(!string.IsNullOrEmpty(credentials)){
        //         string[] _array = credentials.Split(':');
        //         string email = _array[0];
        //         string password = _array[1];
        //         var _user = _dbcontext.User.FirstOrDefault(o => o.Email_id == email);
                
        //         if(_user == null){
        //             return AuthenticateResult.Fail("Email invalid");
        //         }
                
        //         var _password = new PasswordHasher<Object>();
        //         var result = _password.VerifyHashedPassword(_user,_user.Password,password);
               
        //        if(result == PasswordVerificationResult.Failed){
        //         return AuthenticateResult.Fail("Invalid password");
        //        }
                   
                
        //         var Claims = new[] {new Claim(ClaimTypes.Name,_user.UserId)};
        //         var Identity = new ClaimsIdentity(Claims,Scheme.Name);
        //         var principal = new ClaimsPrincipal(Identity);
        //         var ticket = new AuthenticationTicket(principal,Scheme.Name);
        //         return AuthenticateResult.Success(ticket);
        //     }
        //     return AuthenticateResult.Fail("Invalid credentials");
        // }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
{
    var headerValue = Request.Headers["Authorization"].ToString();
    if (string.IsNullOrEmpty(headerValue) || !headerValue.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
    {
        // No credentials provided or invalid format
        return AuthenticateResult.Fail("Invalid credentials");
    }

    var base64Credentials = headerValue.Substring("Basic ".Length).Trim();
    var credentialsBytes = Convert.FromBase64String(base64Credentials);
    var credentials = Encoding.UTF8.GetString(credentialsBytes);
    var credentialsArray = credentials.Split(':', 2); // Limit split to 2 parts

    if (credentialsArray.Length != 2)
    {
        // Invalid credentials format
        return AuthenticateResult.Fail("Invalid credentials");
    }

    var email = credentialsArray[0];
    var password = credentialsArray[1];

    var user = _dbcontext.User.FirstOrDefault(o => o.Email_id == email);
    if (user == null)
    {
        // User not found
        return AuthenticateResult.Fail("Email invalid");
    }

    var passwordHasher = new PasswordHasher<object>(); // Adjust type parameter as needed
    var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
    if (result != PasswordVerificationResult.Success)
    {
        // Invalid password
        return AuthenticateResult.Fail(credentials);
    }

    var claims = new[] { new Claim(ClaimTypes.Name, user.UserId.ToString()) };
    var identity = new ClaimsIdentity(claims, Scheme.Name);
    var principal = new ClaimsPrincipal(identity);
    var ticket = new AuthenticationTicket(principal, Scheme.Name);

    return AuthenticateResult.Success(ticket);
}

    }
}