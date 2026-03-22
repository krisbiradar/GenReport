namespace GenReport.Infrastructure.InMemory.Enums
{
    /// <summary>
    /// Supported AI providers in GenReport.
    /// </summary>
    public enum AiProvider
    {
        OpenAI,
        Anthropic,
        Gemini,

        /// <summary>Ollama runs locally and can serve many underlying open-source models.</summary>
        Ollama
    }

    /// <summary>
    /// String constants that map each <see cref="AiProvider"/> to its OpenRouter model-id prefix.
    /// Ollama is local and has no OpenRouter prefix.
    /// </summary>
    public static class AiProviderConstants
    {
        public const string OpenAI    = "openai";
        public const string Anthropic = "anthropic";
        public const string Gemini    = "google";
        // Ollama — local runner, no OpenRouter prefix

        /// <summary>
        /// Returns the OpenRouter model-id prefix for hosted providers,
        /// or <c>null</c> for local providers (e.g. Ollama).
        /// </summary>
        public static string? GetOpenRouterPrefix(AiProvider provider) => provider switch
        {
            AiProvider.OpenAI    => OpenAI,
            AiProvider.Anthropic => Anthropic,
            AiProvider.Gemini    => Gemini,
            AiProvider.Ollama    => null,
            _                    => null
        };

        /// <summary>
        /// Parses a lowercase provider string (as used in <see cref="ChatCompletionFactory"/>)
        /// into its corresponding <see cref="AiProvider"/> enum value.
        /// </summary>
        public static AiProvider? FromString(string provider) =>
            provider.Trim().ToLowerInvariant() switch
            {
                OpenAI    => AiProvider.OpenAI,
                Anthropic => AiProvider.Anthropic,
                "gemini"  => AiProvider.Gemini,
                Gemini    => AiProvider.Gemini,   // "google"
                "ollama"  => AiProvider.Ollama,
                _         => null
            };
    }
}
