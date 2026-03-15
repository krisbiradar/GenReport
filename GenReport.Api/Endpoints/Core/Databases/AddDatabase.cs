using FastEndpoints;
using GenReport.DB.Domain.Entities.Core;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.HttpRequests.Core.Databases;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GenReport.Api.Endpoints.Core.Databases
{
    public class AddDatabase(ApplicationDbContext context, ILogger<AddDatabase> logger, ICurrentUserService currentUserService) : Endpoint<AddDatabaseRequest, HttpResponse<string>>
    {
        public override void Configure()
        {
            Post("/databases");
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

            // Assign DbProvider. If none exists, this will fail or we can create one dynamically. For now, fetch first matching DbProvider.
            var provider = await context.DbProviders.FirstOrDefaultAsync(p => p.DbType == req.Type, ct);
            if (provider == null)
            {
                logger.LogWarning($"No DbProvider found for Type: {req.Type}. Falling back to default or creating.");
                provider = await context.DbProviders.FirstOrDefaultAsync(ct); // MVP Fallback 
                if (provider == null)
                {
                     await SendAsync(new HttpResponse<string>(HttpStatusCode.BadRequest, "No matching DbProvider configured in the system.", "ERR_BAD_REQUEST", []), cancellation: ct);
                     return;
                }
            }

            var newDatabase = new Database
            {
                Name = req.Name,
                Type = req.Type,
                ConnectionString = req.ConnectionString,
                ServerAddress = req.ServerAddress ?? string.Empty,
                Port = req.Port,
                Username = req.Username ?? string.Empty,
                Password = req.Password ?? string.Empty,
                Description = req.Description ?? string.Empty,
                Status = "Active",
                SizeInBytes = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DbProviderId = provider.Id
            };

            await context.Databases.AddAsync(newDatabase, ct);
            await context.SaveChangesAsync(ct);

            await SendAsync(new HttpResponse<string>("Success", "Database connection successfully added.", HttpStatusCode.OK), cancellation: ct);
        }
    }
}
