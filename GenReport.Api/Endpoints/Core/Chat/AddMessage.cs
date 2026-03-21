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
    public class AddMessage(ApplicationDbContext context, ICurrentUserService currentUserService) : Endpoint<AddMessageRequest, HttpResponse<ChatMessage>>
    {
        public override void Configure()
        {
            Post("/chat/sessions/{id}/messages");
        }

        public override async Task HandleAsync(AddMessageRequest req, CancellationToken ct)
        {
            var sessionId = Route<long>("id");
            var userId = currentUserService.LoggedInUserId();

            var session = await context.ChatSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId, ct);

            if (session == null)
            {
                await SendAsync(new HttpResponse<ChatMessage>(HttpStatusCode.NotFound, "Chat session not found or access denied.", "ERR_NOT_FOUND", []), cancellation: ct);
                return;
            }

            var message = new ChatMessage
            {
                SessionId = sessionId,
                Role = req.Role,
                Content = req.Content
            };

            context.ChatMessages.Add(message);
            session.UpdatedAt = DateTime.UtcNow; // Touch session

            await context.SaveChangesAsync(ct);

            await SendAsync(new HttpResponse<ChatMessage>(message, "Message added successfully", HttpStatusCode.OK), cancellation: ct);
        }
    }
}
