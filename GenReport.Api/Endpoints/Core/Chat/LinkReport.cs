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
    public class LinkReport(ApplicationDbContext context, ICurrentUserService currentUserService) : Endpoint<LinkReportRequest, HttpResponse<MessageReport>>
    {
        public override void Configure()
        {
            Post("/chat/messages/{id}/report");
        }

        public override async Task HandleAsync(LinkReportRequest req, CancellationToken ct)
        {
            var messageId = Route<long>("id");
            var userId = currentUserService.LoggedInUserId();

            // Verify message exists and user has access via session
            var message = await context.ChatMessages
                .Include(m => m.Session)
                .FirstOrDefaultAsync(m => m.Id == messageId, ct);

            if (message == null || message.Session.UserId != userId)
            {
                await SendAsync(new HttpResponse<MessageReport>(HttpStatusCode.NotFound, "Message not found or access denied.", "ERR_NOT_FOUND", []), cancellation: ct);
                return;
            }

            // Verify report exists
            var report = await context.Reports.FirstOrDefaultAsync(r => r.Id == req.ReportId, ct);
            if (report == null)
            {
                await SendAsync(new HttpResponse<MessageReport>(HttpStatusCode.NotFound, "Report not found.", "ERR_NOT_FOUND", []), cancellation: ct);
                return;
            }

            var messageReport = new MessageReport
            {
                MessageId = messageId,
                ReportId = req.ReportId
            };

            context.MessageReports.Add(messageReport);
            await context.SaveChangesAsync(ct);

            await SendAsync(new HttpResponse<MessageReport>(messageReport, "Report linked to message successfully", HttpStatusCode.OK), cancellation: ct);
        }
    }
}
