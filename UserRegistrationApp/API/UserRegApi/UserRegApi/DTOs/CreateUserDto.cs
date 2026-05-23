using System.ComponentModel.DataAnnotations;

namespace UserRegApi.DTOs
{
    public class CreateUserDto
    {

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public int  Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(4)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
        public string ConfirmPassword { get; set; }=string.Empty;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public string Hobby { get; set; } = string.Empty;

        public bool Status { get; set; } =false;

    }
}
