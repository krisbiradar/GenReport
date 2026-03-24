using GenReport.Infrastructure.InMemory.Enums;

namespace GenReport.Infrastructure.InMemory.Models
{
    /// <summary>
    /// Represents a single AI model available for a given provider.
    /// </summary>
    /// <param name="Provider">The AI provider this model belongs to.</param>
    /// <param name="ModelId">The model identifier without the provider prefix (e.g. "gpt-4o-mini").</param>
    /// <param name="ModelName">A human-readable display name (e.g. "OpenAI: GPT-4o Mini").</param>
    public record ProviderModelInfo(AiProvider Provider, string ModelId, string ModelName);
}
