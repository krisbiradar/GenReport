using FastEndpoints;
using GenReport.Infrastructure.Interfaces;
using GenReport.Infrastructure.Models.HttpRequests.Core.Ai;
using GenReport.Infrastructure.Models.Shared;
using System.Net;

namespace GenReport.Api.Endpoints.Core.Ai
{
    /// <summary>
    /// POST /ai/connections/test
    /// Accepts connection details in the request body and sends a lightweight
    /// test prompt to the configured AI provider.
    /// Works for both saved and unsaved connections.
    /// </summary>
    public class TestAiConnection(
        ITestAiConnectionService testAiConnectionService,
        ILogger<TestAiConnection> logger) : Endpoint<TestAiConnectionRequest, HttpResponse<string>>
    {
        public override void Configure()
        {
            Post("/ai/connections/test");
        }

        public override async Task HandleAsync(TestAiConnectionRequest req, CancellationToken ct)
        {
            logger.LogInformation("Testing AI connection for provider {Provider}, model {Model}", req.Provider, req.DefaultModel);

            var result = await testAiConnectionService.TestConnectionAsync(req, ct);

            if (!result.IsSuccess)
            {
                logger.LogWarning("AI connection test failed: {Message}", result.Message);
                await SendAsync(
                    new HttpResponse<string>(HttpStatusCode.BadRequest, "AI connection test failed.", "ERR_AI_TEST_FAILED", [result.Message]),
                    cancellation: ct);
                return;
            }

            await SendAsync(
                new HttpResponse<string>("Success", result.Message, HttpStatusCode.OK),
                cancellation: ct);
        }
    }
}
