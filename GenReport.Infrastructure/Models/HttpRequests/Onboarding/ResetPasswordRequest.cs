using System.Text.Json.Serialization;

namespace GenReport.Infrastructure.Models.HttpRequests.Onboarding
{
    /// <summary>
    /// Request model for the reset-password endpoint
    /// </summary>
    public class ResetPasswordRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("otp")]
        public required string Otp { get; set; }

        [JsonPropertyName("newPassword")]
        public required string NewPassword { get; set; }

        [JsonPropertyName("confirmPassword")]
        public required string ConfirmPassword { get; set; }
    }
}
