using GenReport.Infrastructure.InMemory.Enums;
using GenReport.Infrastructure.InMemory.Models;

namespace GenReport.Infrastructure.InMemory
{
    /// <summary>
    /// Read-only view of the in-memory AI store.
    /// Populated at startup by <see cref="InMemoryAiSeeder"/>.
    /// </summary>
    public interface IInMemoryAiStore
    {
        /// <summary>Returns all model IDs available for the given provider.</summary>
        IReadOnlyList<ProviderModelInfo> GetModelsForProvider(AiProvider provider);

        /// <summary>Returns all supported AI providers.</summary>
        IReadOnlyList<AiProvider> GetSupportedProviders();

    }
}
