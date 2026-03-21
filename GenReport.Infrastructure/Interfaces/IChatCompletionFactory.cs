using Microsoft.SemanticKernel.ChatCompletion;

namespace GenReport.Infrastructure.Interfaces
{
    /// <summary>
    /// Factory that builds the correct <see cref="IChatCompletionService"/>
    /// for a given AI provider (OpenAI, Anthropic, Gemini, Ollama, Custom, etc.).
    /// </summary>
    public interface IChatCompletionFactory
    {
        /// <summary>
        /// Creates an <see cref="IChatCompletionService"/> configured for the
        /// specified provider, API key, model, and optional endpoint URL.
        /// </summary>
        IChatCompletionService Create(string provider, string apiKey, string model, string? chatEndpointUrl = null);
    }
}
