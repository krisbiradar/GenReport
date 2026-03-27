using FastEndpoints;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.InMemory;
using GenReport.Infrastructure.InMemory.Enums;
using GenReport.Infrastructure.Models.HttpResponse.Core.Ai;
using GenReport.Infrastructure.Models.HttpResponse.Core.Chat;
using GenReport.Infrastructure.Models.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GenReport.Api.Endpoints.Core.Chat
{
    /// <summary>
    /// GET /chat/models
    /// Fetches all models the user has access to by getting available active providers 
    /// from the DB and their available models from the in-memory store.
    /// </summary>
    public class GetAvailableModels(
        IInMemoryAiStore aiStore, 
        ApplicationDbContext context, 
        ILogger<GetAvailableModels> logger)
        : EndpointWithoutRequest<HttpResponse<List<ChatProviderModelsResponse>>>
    {
        public override void Configure()
        {
            Get("/chat/models");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            // 1. Fetch available active providers from the DB
            var activeProviders = await context.AiConnections
                .Where(c => c.IsActive)
                .Select(c => c.Provider)
                .Distinct()
                .ToListAsync(ct);

            var result = new List<ChatProviderModelsResponse>();

            // 2. For each active provider, fetch available models from the in-memory store
            foreach (var providerStr in activeProviders)
            {
                var providerEnum = AiProviderConstants.FromString(providerStr);

                if (providerEnum != null)
                {
                    var models = aiStore
                        .GetModelsForProvider(providerEnum.Value)
                        .Select(m => new ProviderModelResponse
                        {
                            ModelId = m.ModelId,
                            ModelName = m.ModelName
                        })
                        .ToList();

                    // Only include providers that actually have configured/supported models
                    if (models.Count != 0)
                    {
                        result.Add(new ChatProviderModelsResponse
                        {
                            Provider = providerStr, // Return original string or enum name as needed
                            Models = models
                        });
                    }
                }
                else
                {
                    logger.LogWarning("Unknown active provider found in DB: {Provider}", providerStr);
                }
            }

            // Return the aggregated list
            await SendAsync(
                new HttpResponse<List<ChatProviderModelsResponse>>(result, "OK", HttpStatusCode.OK),
                cancellation: ct);
        }
    }
}
