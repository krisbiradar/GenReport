using FastEndpoints;
using GenReport.Infrastructure.InMemory;
using GenReport.Infrastructure.InMemory.Enums;
using GenReport.Infrastructure.Models.HttpResponse.Core.Ai;
using GenReport.Infrastructure.Models.Shared;
using System.Net;

namespace GenReport.Api.Endpoints.Core.Ai
{
    /// <summary>
    /// GET /ai/providers/{provider}/models
    /// Returns the list of available models for the given AI provider
    /// from the in-memory store (populated at startup from OpenRouter).
    /// </summary>
    public class GetProviderModels(IInMemoryAiStore aiStore, ILogger<GetProviderModels> logger)
        : EndpointWithoutRequest<HttpResponse<List<ProviderModelResponse>>>
    {
        public override void Configure()
        {
            Get("/ai/providers/{provider}/models");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var providerParam = Route<string>("provider");

            var provider = AiProviderConstants.FromString(providerParam ?? string.Empty);

            if (provider is null)
            {
                logger.LogWarning("Unknown provider requested: {Provider}", providerParam);

                var supported = string.Join(", ", Enum.GetNames<AiProvider>()).ToLowerInvariant();
                await SendAsync(
                    new HttpResponse<List<ProviderModelResponse>>(
                        HttpStatusCode.BadRequest,
                        $"Unknown provider '{providerParam}'. Supported: {supported}",
                        "INVALID_PROVIDER"),
                    (int)HttpStatusCode.BadRequest,
                    cancellation: ct);
                return;
            }

            var models = aiStore
                .GetModelsForProvider(provider.Value)
                .Select(m => new ProviderModelResponse
                {
                    ModelId   = m.ModelId,
                    ModelName = m.ModelName
                })
                .ToList();

            logger.LogInformation(
                "Returning {Count} models for provider {Provider}", models.Count, provider);

            await SendAsync(
                new HttpResponse<List<ProviderModelResponse>>(models, "OK", HttpStatusCode.OK),
                cancellation: ct);
        }
    }
}
