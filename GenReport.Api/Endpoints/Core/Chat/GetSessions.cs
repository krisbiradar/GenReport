using FastEndpoints;
using GenReport.DB.Domain.Entities.Core;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GenReport.Api.Endpoints.Core.Chat
{
    public class GetSessions(ApplicationDbContext context, ICurrentUserService currentUserService) : EndpointWithoutRequest<HttpResponse<List<ChatSession>>>
    {
        public override void Configure()
        {
            Get("/chat/sessions");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var userId = currentUserService.LoggedInUserId();

            var sessions = await context.ChatSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.UpdatedAt)
                .ToListAsync(ct);

            await SendAsync(new HttpResponse<List<ChatSession>>(sessions, "Sessions fetched successfully", HttpStatusCode.OK), cancellation: ct);
        }
    }
}
