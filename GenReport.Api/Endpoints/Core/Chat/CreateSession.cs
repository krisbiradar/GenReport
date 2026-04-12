using FastEndpoints;
using GenReport.DB.Domain.Entities.Core;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.HttpRequests.Core.Chat;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Infrastructure.Static.Constants;
using GenReport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GenReport.Api.Endpoints.Core.Chat
{
    public class CreateSession(ApplicationDbContext context, ICurrentUserService currentUserService) : Endpoint<CreateSessionRequest, HttpResponse<ChatSession>>
    {
        public override void Configure()
        {
            Post("/chat/sessions");
        }

        public override async Task HandleAsync(CreateSessionRequest req, CancellationToken ct)
        {
            var userId = currentUserService.LoggedInUserId();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user == null)
            {
                await SendAsync(new HttpResponse<ChatSession>(HttpStatusCode.Unauthorized, "Unauthorized", "ERR_UNAUTHORIZED", []), cancellation: ct);
                return;
            }

            // Validate prerequisites: at least 1 active AI connection and 1 database must exist
            var hasAiConnection = await context.AiConnections.AnyAsync(a => a.IsActive, ct);
            var hasDatabase = await context.Databases.AnyAsync(ct);

            if (!hasAiConnection || !hasDatabase)
            {
                var missing = new List<string>();

                if (!hasAiConnection)
                    missing.Add("No active AI configuration found. Please add at least one AI configuration before creating a session.");

                if (!hasDatabase)
                    missing.Add("No database found. Please add at least one database before creating a session.");

                await SendAsync(
                    new HttpResponse<ChatSession>(
                        HttpStatusCode.UnprocessableEntity,
                        "Cannot create session: required configuration is missing.",
                        ErrorMessages.SESSION_PREREQUISITES_NOT_MET,
                        missing),
                    cancellation: ct);
                return;
            }

            // Resolve the default AI connection to pre-populate the session's provider
            var defaultConnection = await context.AiConnections
                .Where(a => a.IsDefault && a.IsActive)
                .FirstOrDefaultAsync(ct);

            var session = new ChatSession
            {
                UserId = userId!.Value,
                Title = req.Title ?? "New Chat",
                ModelId = req.ModelId,
                AiConnectionId = defaultConnection?.Id,
                DatabaseId = long.TryParse(req.DatabaseConnectionId, out var databaseId) ? databaseId : null
            };

            context.ChatSessions.Add(session);
            await context.SaveChangesAsync(ct);

            await SendAsync(new HttpResponse<ChatSession>(session, "Session created successfully", HttpStatusCode.OK), cancellation: ct);
        }
    }
}
