using FastEndpoints;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.HttpRequests.Core.Databases;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;
using GenReport.Infrastructure.Interfaces;

namespace GenReport.Api.Endpoints.Core.Databases
{
    public class TestConnection(ApplicationDbContext context, ILogger<TestConnection> logger, ICurrentUserService currentUserService, ITestConnectionService testConnectionService) : Endpoint<AddDatabaseRequest, HttpResponse<string>>
    {
        public override void Configure()
        {
            Post("/connections/test");
        }

        public override async Task HandleAsync(AddDatabaseRequest req, CancellationToken ct)
        {
            var userId = currentUserService.LoggedInUserId();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user == null)
            {
                logger.LogError("Invalid user context");
                await SendAsync(new HttpResponse<string>(HttpStatusCode.Unauthorized, "Unauthorized", "ERR_UNAUTHORIZED", []), cancellation: ct);
                return;
            }

            logger.LogInformation($"Testing connection to {req.HostName}:{req.Port} for connection name {req.Name}");
            var connectionTestResult = await testConnectionService.TestConnectionAsync(req, ct);
            if (!connectionTestResult.IsSuccess)
            {
                logger.LogError("Database connection test failed for {HostName}:{Port} - {Reason}", req.HostName, req.Port, connectionTestResult.Message);
                await SendAsync(new HttpResponse<string>(HttpStatusCode.BadRequest, "Database connection test failed.", "ERR_CONNECTION_TEST_FAILED", [connectionTestResult.Message]), cancellation: ct);
                return;
            }

            await SendAsync(new HttpResponse<string>("Success", connectionTestResult.Message, HttpStatusCode.OK), cancellation: ct);
        }
    }
}
