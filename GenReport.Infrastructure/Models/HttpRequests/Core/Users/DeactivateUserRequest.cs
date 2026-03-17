using System.ComponentModel.DataAnnotations;

namespace GenReport.Infrastructure.Models.HttpRequests.Core.Users
{
    public class DeactivateUserRequest
    {
        [Required]
        public long UserId { get; set; }
    }
}
