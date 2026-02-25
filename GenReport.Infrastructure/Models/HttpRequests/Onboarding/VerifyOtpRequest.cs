using System.Text.Json.Serialization;

namespace GenReport.Infrastructure.Models.HttpRequests.Onboarding
{
    /// <summary>
    /// Request model for the verify-otp endpoint
    /// </summary>
    public class VerifyOtpRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("otp")]
        public required string Otp { get; set; }
    }
}
