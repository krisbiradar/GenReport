using System.Collections.Concurrent;
using GenReport.Infrastructure.InMemory.Enums;
using GenReport.Infrastructure.InMemory.Models;

namespace GenReport.Infrastructure.InMemory
{
    /// <summary>
    /// Thread-safe singleton in-memory AI store.
    /// Reading is done via <see cref="IInMemoryAiStore"/>.
    /// Writing is internal — only <see cref="InMemoryAiSeeder"/> should call the setter methods.
    /// </summary>
    public sealed class InMemoryAiStore : IInMemoryAiStore
    {
        private readonly ConcurrentDictionary<AiProvider, List<ProviderModelInfo>> _models
            = new();

        private readonly ConcurrentDictionary<AiProvider, ProviderDefaultConfig> _defaultConfigs
            = new();

        // ── IInMemoryAiStore (read) ──────────────────────────────────────────────

        public IReadOnlyList<ProviderModelInfo> GetModelsForProvider(AiProvider provider)
            => _models.TryGetValue(provider, out var list)
                ? list.AsReadOnly()
                : [];

        public IReadOnlyList<AiProvider> GetSupportedProviders()
            => [.. Enum.GetValues<AiProvider>()];

        public ProviderDefaultConfig? GetDefaultConfig(AiProvider provider)
            => _defaultConfigs.TryGetValue(provider, out var config) ? config : null;

        // ── Internal write methods (called only by InMemoryAiSeeder) ─────────────

        internal void SetModels(AiProvider provider, IEnumerable<ProviderModelInfo> models)
            => _models[provider] = [.. models];

        internal void SetDefaultConfig(AiProvider provider, ProviderDefaultConfig config)
            => _defaultConfigs[provider] = config;
    }
}
