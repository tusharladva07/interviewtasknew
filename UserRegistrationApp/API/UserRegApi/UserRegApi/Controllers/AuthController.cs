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
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
            var user = await _context.Users
                .FirstOrDefaultAsync(t => t.Email.ToLower() == normalizedEmail && t.IsDeleted != true);

            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid email or password" });
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return Unauthorized(new { Message = "Invalid email or password" });
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user.Name,
                user.Email
            });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var minutes = Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "10");
            var jwtKey = _configuration["Jwt:Key"]
                ?? Environment.GetEnvironmentVariable("JWT_KEY")
                ?? throw new InvalidOperationException("JWT signing key is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
