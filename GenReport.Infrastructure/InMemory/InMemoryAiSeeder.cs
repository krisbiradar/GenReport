using System.Text.Json;
using System.Text.Json.Serialization;
using GenReport.Infrastructure.InMemory.Enums;
using GenReport.Infrastructure.InMemory.Models;
using Microsoft.Extensions.Logging;

namespace GenReport.Infrastructure.InMemory
{
    /// <summary>
    /// Seeds the <see cref="InMemoryAiStore"/> at application startup:
    /// <list type="bullet">
    ///   <item>Fetches available models from OpenRouter and filters to supported providers.</item>
    ///   <item>Seeds hard-coded default connection configs for each provider.</item>
    /// </list>
    /// </summary>
    public sealed class InMemoryAiSeeder(
        InMemoryAiStore store,
        IHttpClientFactory httpClientFactory,
        ILogger<InMemoryAiSeeder> logger)
    {
        private const string OpenRouterModelsUrl = "https://openrouter.ai/api/v1/models";

        // ── Seed entry point ─────────────────────────────────────────────────────

        public async Task SeedAsync(CancellationToken ct = default)
        {
            // 1. Seed Ollama with empty model list (models are dynamic/local)
            store.SetModels(AiProvider.Ollama, []);
            logger.LogInformation("[InMemoryAiSeeder] Ollama is local — model list left empty");

            // 2. Fetch OpenRouter models for hosted providers
            await FetchAndSeedOpenRouterModelsAsync(ct);
        }

        // ── OpenRouter fetch ─────────────────────────────────────────────────────

        private async Task FetchAndSeedOpenRouterModelsAsync(CancellationToken ct)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("User-Agent", "GenReport/1.0");

                var response = await client.GetAsync(OpenRouterModelsUrl, ct);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(ct);
                var result = JsonSerializer.Deserialize<OpenRouterModelsResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result?.Data is null)
                {
                    logger.LogWarning("[InMemoryAiSeeder] OpenRouter returned no model data");
                    EnsureEmptyModelLists();
                    return;
                }

                // Filter by known provider prefixes
                var hostedProviders = new[]
                {
                    AiProvider.OpenAI,
                    AiProvider.Anthropic,
                    AiProvider.Gemini
                };

                foreach (var provider in hostedProviders)
                {
                    var prefix = AiProviderConstants.GetOpenRouterPrefix(provider);
                    if (prefix is null) continue;

                    var models = result.Data
                        .Where(m => m.Id.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase))
                        .Select(m => new ProviderModelInfo(provider, m.Id[(prefix.Length + 1)..], m.Name))
                        .ToList();

                    store.SetModels(provider, models);
                    logger.LogInformation("[InMemoryAiSeeder] Seeded {Count} {Provider} models", models.Count, provider);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "[InMemoryAiSeeder] Failed to fetch models from OpenRouter — starting with empty model lists");
                EnsureEmptyModelLists();
            }
        }

        private void EnsureEmptyModelLists()
        {
            foreach (AiProvider provider in Enum.GetValues<AiProvider>())
            {
                // Only overwrite if not already set (Ollama is already empty)
                if (store.GetModelsForProvider(provider).Count == 0)
                    store.SetModels(provider, []);
            }
        }

        // ── OpenRouter response DTOs (minimal) ───────────────────────────────────

        private sealed class OpenRouterModelsResponse
        {
            public List<OpenRouterModel> Data { get; set; } = [];
        }

        private sealed class OpenRouterModel
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
        }
    }
}
