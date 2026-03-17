using System.ComponentModel.DataAnnotations;

namespace GenReport.Infrastructure.Models.HttpRequests.Core.Users
{
    public class CreateUserRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        public string? MiddleName { get; set; }

        public string? ProfileURL { get; set; }

        [Required]
        [MinLength(8)]
        public required string Password { get; set; }

        public int RoleId { get; set; } = 1;
    }
}
