namespace GenReport.Infrastructure.Models.HttpResponse.Onboarding
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Defines the <see cref="LoginResponse" />
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets the Token
        /// </summary>
        [JsonPropertyName("token")]
        public required string Token { get; set; }

        /// <summary>
        /// Gets or sets the RefreshToken
        /// </summary>
        [JsonPropertyName("refreshtoken")]
        public required string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the Role
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; } = "user";

        /// <summary>
        /// Gets or sets the Email
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = "";

        /// <summary>
        /// Gets or sets the FirstName
        /// </summary>
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = "";

        /// <summary>
        /// Gets or sets the LastName
        /// </summary>
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = "";
    }
}
