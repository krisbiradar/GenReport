using FastEndpoints;
using GenReport.DB.Domain.Entities.Core;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GenReport.Api.Endpoints.Core.Chat
{
    public class GetSession(ApplicationDbContext context, ICurrentUserService currentUserService) : EndpointWithoutRequest<HttpResponse<ChatSession>>
    {
        public override void Configure()
        {
            Get("/chat/sessions/{id}");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<long>("id");
            var userId = currentUserService.LoggedInUserId();

            var session = await context.ChatSessions
                .Include(s => s.Messages)
                    .ThenInclude(m => m.Reports)
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId, ct);

            if (session == null)
            {
                await SendAsync(new HttpResponse<ChatSession>(HttpStatusCode.NotFound, "Chat session not found.", "ERR_NOT_FOUND", []), cancellation: ct);
                return;
            }

            // Client usually expects messages ordered by time ascending
            session.Messages = session.Messages.OrderBy(m => m.CreatedAt).ToList();

            await SendAsync(new HttpResponse<ChatSession>(session, "Session fetched successfully", HttpStatusCode.OK), cancellation: ct);
        }
    }
}
