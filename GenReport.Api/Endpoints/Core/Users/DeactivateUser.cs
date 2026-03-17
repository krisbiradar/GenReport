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
    public class DeactivateUser(ApplicationDbContext context, ILogger<DeactivateUser> logger, ICurrentUserService currentUserService) : Endpoint<DeactivateUserRequest, HttpResponse<string>>
    {
        public override void Configure()
        {
            Put("/users/deactivate");
        }

        public override async Task HandleAsync(DeactivateUserRequest req, CancellationToken ct)
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

            if (existingUser.Id == currentUser.Id)
            {
                await SendAsync(new HttpResponse<string>(HttpStatusCode.BadRequest, "You cannot deactivate your own account.", "ERR_SELF_DEACTIVATE", []), cancellation: ct);
                return;
            }

            existingUser.IsDeleted = true;
            existingUser.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(ct);

            await SendAsync(new HttpResponse<string>("Success", "User deactivated successfully.", HttpStatusCode.OK), cancellation: ct);
        }
    }
}
