using GenReport.Infrastructure.InMemory.Enums;

namespace GenReport.Infrastructure.InMemory.Models
{
    /// <summary>
    /// Default connection configuration for a given AI provider,
    /// used as the baseline when testing an AI connection.
    /// </summary>
    /// <param name="Provider">The AI provider.</param>
    /// <param name="DefaultModel">The default model ID to use for this provider.</param>
    /// <param name="ChatEndpointUrl">
    /// Override endpoint URL. <c>null</c> for hosted providers that use their native SDK
    /// (OpenAI, Anthropic, Gemini). Set to the Ollama local URL for Ollama.
    /// </param>
    public record ProviderDefaultConfig(
        AiProvider Provider,
        string DefaultModel,
        string? ChatEndpointUrl
    );
}
