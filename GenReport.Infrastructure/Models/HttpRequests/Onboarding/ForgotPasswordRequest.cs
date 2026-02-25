using System.Text.Json.Serialization;

namespace GenReport.Infrastructure.Models.HttpRequests.Onboarding
{
    /// <summary>
    /// Request model for the forgot-password endpoint
    /// </summary>
    public class ForgotPasswordRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }
    }
}
