namespace GenReport.Infrastructure.Models.HttpRequests.Onboarding
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Defines the <see cref="LoginRequest" />
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the Email
        /// </summary>
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the Password
        /// </summary>
        [JsonPropertyName("password")]
        public required string Password { get; set; }
    }
}
