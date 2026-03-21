#pragma warning disable SKEXP0010 // Semantic Kernel experimental API — custom endpoint constructor
#pragma warning disable SKEXP0070 // Semantic Kernel experimental API — Google AI connector

using GenReport.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace GenReport.Infrastructure.SharedServices.Core.Ai
{
    /// <summary>
    /// Resolves the correct <see cref="IChatCompletionService"/> for a given AI provider.
    /// - OpenAI: <see cref="OpenAIChatCompletionService"/> (Semantic Kernel)
    /// - Gemini: <see cref="GoogleAIGeminiChatCompletionService"/> (Semantic Kernel)
    /// - Anthropic: <see cref="AnthropicChatCompletionAdapter"/> (Anthropic.SDK)
    /// - Ollama / Custom: OpenAI-compatible via SK with custom endpoint
    /// </summary>
    public sealed class ChatCompletionFactory(ILogger<ChatCompletionFactory> logger) : IChatCompletionFactory
    {
        private static readonly Dictionary<string, string> DefaultProviderEndpoints = new(StringComparer.OrdinalIgnoreCase)
        {
            ["ollama"] = "http://localhost:11434/v1",
        };

        public IChatCompletionService Create(string provider, string apiKey, string model, string? chatEndpointUrl = null)
        {
            logger.LogInformation("Creating chat completion service for provider {Provider}, model {Model}", provider, model);

            var normalisedProvider = provider.Trim().ToLowerInvariant();

            return normalisedProvider switch
            {
                // ── OpenAI ──────────────────────────────────────────
                "openai" => new OpenAIChatCompletionService(
                    modelId: model,
                    apiKey: apiKey),

                // ── Google Gemini (native SK connector) ─────────────
                "gemini" => new GoogleAIGeminiChatCompletionService(
                    modelId: model,
                    apiKey: apiKey),

                // ── Anthropic (native SDK via adapter) ──────────────
                "anthropic" => new AnthropicChatCompletionAdapter(apiKey, model),

                // ── Ollama / Custom (OpenAI-compatible) ─────────────
                _ => CreateOpenAiCompatible(normalisedProvider, model, apiKey, chatEndpointUrl)
            };
        }

        private OpenAIChatCompletionService CreateOpenAiCompatible(
            string provider, string model, string apiKey, string? chatEndpointUrl)
        {
            var endpointUrl = chatEndpointUrl
                ?? (DefaultProviderEndpoints.TryGetValue(provider, out var defaultUrl) ? defaultUrl : null);

            if (string.IsNullOrWhiteSpace(endpointUrl))
            {
                throw new InvalidOperationException(
                    $"Provider '{provider}' requires a chat endpoint URL, but none was provided " +
                    $"and no known default exists. Please supply a chatEndpointUrl.");
            }

            logger.LogInformation("Using OpenAI-compatible endpoint for provider {Provider}: {Url}", provider, endpointUrl);

            return new OpenAIChatCompletionService(
                modelId: model,
                apiKey: apiKey,
                endpoint: new Uri(endpointUrl));
        }
    }
}
