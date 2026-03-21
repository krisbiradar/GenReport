namespace GenReport.Infrastructure.Models.HttpRequests.Core.Ai
{
    /// <summary>
    /// Request model for testing an AI connection.
    /// Accepts all connection details so the connection need not exist in the DB yet.
    /// </summary>
    public class TestAiConnectionRequest
    {
        /// <summary>Provider name (e.g. OpenAI, Anthropic, Gemini, Ollama, Custom).</summary>
        public required string Provider { get; set; }

        /// <summary>Raw (unencrypted) API key.</summary>
        public required string ApiKey { get; set; }

        /// <summary>Model identifier (e.g. gpt-4o, claude-3-5-sonnet).</summary>
        public required string DefaultModel { get; set; }

        /// <summary>
        /// Optional chat endpoint URL for non-OpenAI providers
        /// (e.g. https://api.anthropic.com/v1/chat/completions).
        /// Required for providers other than OpenAI.
        /// </summary>
        public string? ChatEndpointUrl { get; set; }
    }
}
