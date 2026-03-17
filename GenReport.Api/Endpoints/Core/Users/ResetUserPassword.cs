using FastEndpoints;
using GenReport.DB.Domain.Enums;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.HttpRequests.Core.Users;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GenReport.Api.Endpoints.Core.Users
{
    public class ResetUserPassword(ApplicationDbContext context, ILogger<ResetUserPassword> logger, ICurrentUserService currentUserService) : Endpoint<ResetUserPasswordRequest, HttpResponse<string>>
    {
        public override void Configure()
        {
            Put("/users/reset-password");
        }

        public override async Task HandleAsync(ResetUserPasswordRequest req, CancellationToken ct)
        {
            var currentUserId = currentUserService.LoggedInUserId();
            var currentUser = await context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId, ct);
            if (currentUser == null)
            {
                logger.LogError("Invalid user context");
                await SendAsync(new HttpResponse<string>(HttpStatusCode.Unauthorized, "Unauthorized", "ERR_UNAUTHORIZED", []), cancellation: ct);
                return;
            }

            if (currentUser.RoleId != (int)Role.Admin)
            {
                await SendAsync(new HttpResponse<string>(HttpStatusCode.Forbidden, "Forbidden", "ERR_FORBIDDEN", []), cancellation: ct);
                return;
            }

            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Id == req.UserId, ct);
            if (existingUser == null)
            {
                await SendAsync(new HttpResponse<string>(HttpStatusCode.NotFound, "User not found.", "ERR_USER_NOT_FOUND", []), cancellation: ct);
                return;
            }

            existingUser.UpdatePassword(req.NewPassword);
            existingUser.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(ct);

            await SendAsync(new HttpResponse<string>("Success", "User password reset successfully.", HttpStatusCode.OK), cancellation: ct);
        }
    }
}
