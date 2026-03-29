namespace GenReport.Infrastructure.Models.AI
{
    /// <summary>
    /// Default lightweight model IDs per provider, used when no ModelOverride is set in LlmConfig.
    /// These are the smallest/cheapest models good enough for intent classification.
    /// </summary>
    public static class LightweightModelMap
    {
        private static readonly Dictionary<string, string> ProviderModels = new(StringComparer.OrdinalIgnoreCase)
        {
            ["gemini"]    = "gemini-2.0-flash-lite",
            ["openai"]    = "gpt-4o-mini",
            ["anthropic"] = "claude-3-5-haiku-20241022",
            ["ollama"]    = "llama3.2:1b",
        };

        /// <summary>
        /// Returns the default lightweight model for the given provider.
        /// Falls back to the provider's default model if no lightweight mapping exists.
        /// </summary>
        public static string GetLightweightModel(string provider, string fallbackModel)
        {
            return ProviderModels.TryGetValue(provider.Trim(), out var model)
                ? model
                : fallbackModel;
        }
    }
}
