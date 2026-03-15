using FastEndpoints;

namespace GenReport.Api.Endpoints.Health
{
    public class Heartbeat : EndpointWithoutRequest
    {
        public override void Configure()
        {
            Get("/api/heartbeat");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await SendAsync("OK", cancellation: ct);
        }
    }
}

