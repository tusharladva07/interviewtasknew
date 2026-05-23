using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserRegApi.Data;
using UserRegApi.DTOs;
using UserRegApi.Models;

namespace UserRegApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration =configuration;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users.FirstOrDefaultAsync(t => t.Email.Equals(dto.Email));

            if(user == null)
            {
                return Unauthorized("Invalid email or Password");
            }
            
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password,user.Password);

            if (isPasswordValid == false)
            {
                return Unauthorized("Invalid email or Password");
            }
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user.Name,
                user.Email
            });
        }

        private object GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Name.ToString()),
                new Claim(ClaimTypes.Email,user.Email.ToString())
            };
            var minutes = Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(

                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(minutes),
                signingCredentials: cred
                );

            return  new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
