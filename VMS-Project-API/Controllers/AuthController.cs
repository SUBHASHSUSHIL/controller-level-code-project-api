using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VMS_Project_API.Entities;
using Microsoft.EntityFrameworkCore;
using VMS_Project_API.Data;
using VMS_Project_API.Model;
using VMS_Project_API.AppCode;
using Microsoft.AspNetCore.Authorization;

namespace VMS_Project_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] Login user)
        {
            if (user is null)
            {
                return BadRequest("Invalid user request!!!");
            }


            var rData = _context.Users.Where(u => u.Username == user.UserName && u.Status==true).FirstOrDefault();

            if (rData != null)
            {

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalModel.JWTSecret));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(issuer: GlobalModel.JWTValidIssuer, audience: GlobalModel.JWTValidAudience, claims: new List<Claim>(), expires: DateTime.Now.AddDays(30), signingCredentials: signinCredentials);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new JWTTokenResponse
                {
                    Token = tokenString
                });

            }
            else
            {
                return Unauthorized();
            }
            
        }
    }

    public class Login
    {
        public string? UserName
        {
            get;
            set;
        }
        public string? Password
        {
            get;
            set;
        }
    }

    public class JWTTokenResponse
    {
        public string? Token
        {
            get;
            set;
        }
    }



}