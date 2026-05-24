namespace UserRegApi.DTOs
{
    public class UserListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Hobby { get; set; } = string.Empty;
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
