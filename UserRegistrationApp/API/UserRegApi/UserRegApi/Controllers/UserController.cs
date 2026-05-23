using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserRegApi.Data;
using UserRegApi.DTOs;
using UserRegApi.Models;

namespace UserRegApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const string DefaultErrorMessage="Internal server error";
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetList")]
        public async Task<ActionResult> GetList()
        {
            try
            {

                var listData = await _context.Users.Where(t=>t.IsDeleted != true).AsNoTracking().ToListAsync();
                return Ok(listData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = DefaultErrorMessage,
                    Error = ex.Message
                });

            }
        }

        [HttpGet("GetDetails/{id}")]
        public async Task<ActionResult> GetDetails(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var userDetails = await _context.Users
                    .Where(x => x.Id == id)
                    .Select(x => new UpdateUserDto
                    {
                        Id = x.Id,
                        Email = x.Email,
                        Name = x.Name,
                        DateOfBirth = x.DateOfBirth,
                        Status = x.Status,
                        Gender = x.Gender,
                        ConfirmPassword = x.Password,
                        Password = x.Password,
                        Hobby = x.Hobby,
                    })
                    .FirstOrDefaultAsync();

                if (userDetails == null)
                {
                    return NotFound("Details Not Found");
                }

                return Ok(userDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = DefaultErrorMessage,
                    Error = ex.Message
                });

            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create(UpdateUserDto userDto)
        {
            try
            {
                if (userDto == null)
                {
                    return BadRequest(new
                    {
                        Message =   "Invalid User Data"
                    });
                }

                bool emailExists = await _context.Users.AnyAsync(x => x.Email == userDto.Email);

                if (emailExists)
                {
                    return BadRequest(new
                    {
                        Message = "Email already exists"
                    });
                }
                User obj = new User();
                obj.Name = userDto.Name;
                obj.Email = userDto.Email;
                obj.Hobby = userDto.Hobby;
                obj.Status= userDto.Status;
                obj.Gender= userDto.Gender;
                obj.DateOfBirth= userDto.DateOfBirth;
                obj.CreatedDate= DateTime.Now;
                obj.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

                await _context.AddAsync(obj);
                await _context.SaveChangesAsync();
                return StatusCode(201,new
                {

                    Message = "User Created Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = DefaultErrorMessage,
                    Error = ex.Message
                });

            }
        }

        [HttpPut("Update/{id:int}")]
        public async Task<ActionResult> Update(int id, UpdateUserDto userDto)
        {
            try
            {
                if (userDto == null)
                {
                    return NotFound();
                }

                var userDetails = await _context.Users.FindAsync(id).ConfigureAwait(false);


                if (userDetails == null)
                {
                    return NotFound("Details Not Found");
                }
                if (!userDto.Password.Equals(userDto.ConfirmPassword))
                {
                    return NotFound("Password is not matching");
                }
                userDetails.Name = userDto.Name;
                userDetails.Email = userDto.Email;
                userDetails.Hobby = userDto.Hobby;
                userDetails.Status = userDto.Status;
                userDetails.Gender = userDto.Gender;
                userDetails.DateOfBirth = userDto.DateOfBirth;
                userDetails.CreatedDate = DateTime.Now;
                userDetails.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

                 _context.Update(userDetails);
                await _context.SaveChangesAsync();
                return StatusCode(200, new
                {

                    Message = "User Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = DefaultErrorMessage,
                    Error = ex.Message
                });

            }
        }
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userDetails = await _context.Users.FindAsync(id);


                if (userDetails == null)
                {
                    return NotFound("Details Not Found");
                }

                userDetails.IsDeleted = true;
                userDetails.UpdatedDate = DateTime.Now;


                _context.Update(userDetails);
                await _context.SaveChangesAsync();
                return StatusCode(200, new
                {

                    Message = "User Deleted Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = DefaultErrorMessage,
                    Error = ex.Message
                });

            }
        }
    }
}
