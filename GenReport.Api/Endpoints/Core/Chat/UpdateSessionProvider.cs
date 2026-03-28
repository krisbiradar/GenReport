using FastEndpoints;
using GenReport.DB.Domain.Entities.Core;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.HttpRequests.Core.Chat;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GenReport.Api.Endpoints.Core.Chat
{
    /// <summary>
    /// PUT /chat/sessions/{id}/provider
    /// Updates the AI provider connection bound to the specified chat session.
    /// </summary>
    public class UpdateSessionProvider(ApplicationDbContext context, ICurrentUserService currentUserService)
        : Endpoint<UpdateSessionProviderRequest, HttpResponse<ChatSession>>
    {
        public override void Configure()
        {
            Put("/chat/sessions/{id}/provider");
        }

        public override async Task HandleAsync(UpdateSessionProviderRequest req, CancellationToken ct)
        {
            var sessionId = Route<long>("id");
            var userId = currentUserService.LoggedInUserId();

            var session = await context.ChatSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId, ct);

            if (session == null)
            {
                await SendAsync(new HttpResponse<ChatSession>(HttpStatusCode.NotFound, "Chat session not found or access denied.", "ERR_NOT_FOUND", []), cancellation: ct);
                return;
            }

            // Verify the requested AiConnection exists and is active
            var aiConnection = await context.AiConnections
                .FirstOrDefaultAsync(a => a.Id == req.AiConnectionId && a.IsActive, ct);

            if (aiConnection == null)
            {
                await SendAsync(new HttpResponse<ChatSession>(HttpStatusCode.BadRequest, "The specified AI connection does not exist or is inactive.", "ERR_INVALID_AI_CONNECTION", []), cancellation: ct);
                return;
            }

            session.AiConnectionId = req.AiConnectionId;
            session.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);

            await SendAsync(new HttpResponse<ChatSession>(session, "Session provider updated successfully", HttpStatusCode.OK), cancellation: ct);
        }
    }
}
