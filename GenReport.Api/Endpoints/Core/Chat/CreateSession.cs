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

            var session = new ChatSession
            {
                UserId = userId.Value,
                Title = req.Title ?? "New Chat"
            };

            context.ChatSessions.Add(session);
            await context.SaveChangesAsync(ct);

            await SendAsync(new HttpResponse<ChatSession>(session, "Session created successfully", HttpStatusCode.OK), cancellation: ct);
        }
    }
}
