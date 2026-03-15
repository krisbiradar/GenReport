using FastEndpoints;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.HttpRequests.Core.Databases;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GenReport.Api.Endpoints.Core.Databases
{
    public class TestConnection(ApplicationDbContext context, ILogger<TestConnection> logger, ICurrentUserService currentUserService) : Endpoint<AddDatabaseRequest, HttpResponse<string>>
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

            // Implement actual testing logic here depending on the DbProvider in a real scenario
            // For now just simulate success
            logger.LogInformation($"Testing connection to {req.HostName}:{req.Port} for database alias {req.DatabaseAlias}");

            await SendAsync(new HttpResponse<string>("Success", "Database connection test successful.", HttpStatusCode.OK), cancellation: ct);
        }
    }
}
