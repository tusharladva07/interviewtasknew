using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserRegApi.Data;
using UserRegApi.DTOs;
using UserRegApi.Models;

namespace UserRegApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;
        private const string DefaultErrorMessage = "Internal server error";

        public UserController(ApplicationDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("GetList")]
        public async Task<ActionResult<IEnumerable<UserListItemDto>>> GetList()
        {
            try
            {
                var listData = await _context.Users
                    .AsNoTracking()
                    .Where(t => t.IsDeleted != true)
                    .Select(x => new UserListItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Email = x.Email,
                        DateOfBirth = x.DateOfBirth,
                        Gender = x.Gender,
                        Hobby = x.Hobby,
                        Status = x.Status,
                        CreatedDate = x.CreatedDate
                    })
                    .ToListAsync();

                return Ok(listData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve user list");
                return StatusCode(500, new { Message = DefaultErrorMessage });
            }
        }

        [HttpGet("GetDetails/{id}")]
        public async Task<ActionResult<UserDetailDto>> GetDetails(int id)
        {
            try
            {
                var userDetails = await _context.Users
                    .AsNoTracking()
                    .Where(x => x.Id == id && x.IsDeleted != true)
                    .Select(x => new UserDetailDto
                    {
                        Id = x.Id,
                        Email = x.Email,
                        Name = x.Name,
                        DateOfBirth = x.DateOfBirth,
                        Status = x.Status,
                        Gender = x.Gender,
                        Hobby = x.Hobby,
                        CreatedDate = x.CreatedDate,
                        UpdatedDate = x.UpdatedDate
                    })
                    .FirstOrDefaultAsync();

                if (userDetails == null)
                {
                    return NotFound(new { Message = "Details not found" });
                }

                return Ok(userDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve user details for {UserId}", id);
                return StatusCode(500, new { Message = DefaultErrorMessage });
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create(CreateUserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var normalizedEmail = userDto.Email.Trim().ToLowerInvariant();
                var emailExists = await _context.Users
                    .AnyAsync(x => x.Email.ToLower() == normalizedEmail && x.IsDeleted != true);

                if (emailExists)
                {
                    return BadRequest(new { Message = "Email already exists" });
                }

                var user = new User
                {
                    Name = userDto.Name.Trim(),
                    Email = userDto.Email.Trim(),
                    Hobby = userDto.Hobby,
                    Status = userDto.Status,
                    Gender = userDto.Gender,
                    DateOfBirth = userDto.DateOfBirth,
                    CreatedDate = DateTime.UtcNow,
                    Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
                };

                await _context.AddAsync(user);
                await _context.SaveChangesAsync();

                return StatusCode(201, new { Message = "User created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user");
                return StatusCode(500, new { Message = DefaultErrorMessage });
            }
        }

        [HttpPut("Update/{id:int}")]
        public async Task<ActionResult> Update(int id, UpdateUserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != userDto.Id)
                {
                    return BadRequest(new { Message = "User id mismatch" });
                }

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted != true);

                if (userDetails == null)
                {
                    return NotFound(new { Message = "Details not found" });
                }

                var normalizedEmail = userDto.Email.Trim().ToLowerInvariant();
                var emailExists = await _context.Users
                    .AnyAsync(x =>
                        x.Id != id &&
                        x.Email.ToLower() == normalizedEmail &&
                        x.IsDeleted != true);

                if (emailExists)
                {
                    return BadRequest(new { Message = "Email already exists" });
                }

                userDetails.Name = userDto.Name.Trim();
                userDetails.Email = userDto.Email.Trim();
                userDetails.Hobby = userDto.Hobby;
                userDetails.Status = userDto.Status;
                userDetails.Gender = userDto.Gender;
                userDetails.DateOfBirth = userDto.DateOfBirth;
                userDetails.UpdatedDate = DateTime.UtcNow;
                userDetails.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

                await _context.SaveChangesAsync();

                return Ok(new { Message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user {UserId}", id);
                return StatusCode(500, new { Message = DefaultErrorMessage });
            }
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted != true);

                if (userDetails == null)
                {
                    return NotFound(new { Message = "Details not found" });
                }

                userDetails.IsDeleted = true;
                userDetails.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { Message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user {UserId}", id);
                return StatusCode(500, new { Message = DefaultErrorMessage });
            }
        }
    }
}
