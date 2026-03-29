using GenReport.Infrastructure.Models.AI;

namespace GenReport.Infrastructure.Interfaces
{
    /// <summary>
    /// Classifies a user message into one of the predefined intents
    /// using a lightweight LLM call with controlled JSON output.
    /// </summary>
    public interface IIntentClassifierService
    {
        /// <summary>
        /// Classifies the user's message into a <see cref="ChatIntent"/>.
        /// </summary>
        /// <param name="userMessage">The raw user message to classify.</param>
        /// <param name="provider">AI provider name (e.g. "gemini", "openai").</param>
        /// <param name="apiKey">Decrypted API key for the provider.</param>
        /// <param name="defaultModel">The default model from AiConnection (used as fallback).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The classification result with intent and confidence.</returns>
        Task<IntentClassificationResult> ClassifyAsync(
            string userMessage,
            string provider,
            string apiKey,
            string defaultModel,
            CancellationToken ct = default);
    }
}
