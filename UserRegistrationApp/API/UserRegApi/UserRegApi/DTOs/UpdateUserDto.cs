using System.ComponentModel.DataAnnotations;

namespace UserRegApi.DTOs
{
    public class UpdateUserDto
    {
            [Required]
            public int Id { get; set; }

            [Required]
            [StringLength(100)]
            public string Name { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [MinLength(4)]
            public string Password { get; set; } = string.Empty;

            [Required]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required]
            public DateOnly DateOfBirth { get; set; }

            [Required]
            public string Gender { get; set; } = string.Empty;

            [Required]
            public string Hobby { get; set; } = string.Empty;

            public bool Status { get; set; } = false;
            public DateTime UpdatedDate { get; set; }

    }
}
