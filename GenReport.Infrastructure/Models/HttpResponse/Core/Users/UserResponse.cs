namespace GenReport.Infrastructure.Models.HttpResponse.Core.Users
{
    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? ProfileURL { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
