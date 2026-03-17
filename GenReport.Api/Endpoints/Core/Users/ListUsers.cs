using FastEndpoints;
using GenReport.DB.Domain.Enums;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.HttpResponse.Core.Users;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GenReport.Api.Endpoints.Core.Users
{
    public class ListUsers(ApplicationDbContext context, ILogger<ListUsers> logger, ICurrentUserService currentUserService) : EndpointWithoutRequest<HttpResponse<List<UserResponse>>>
    {
        public override void Configure()
        {
            Get("/users");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var currentUserId = currentUserService.LoggedInUserId();
            var currentUser = await context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId, ct);
            if (currentUser == null)
            {
                logger.LogError("Invalid user context");
                await SendAsync(new HttpResponse<List<UserResponse>>(HttpStatusCode.Unauthorized, "Unauthorized", "ERR_UNAUTHORIZED", []), cancellation: ct);
                return;
            }

            if (currentUser.RoleId != (int)Role.Admin)
            {
                await SendAsync(new HttpResponse<List<UserResponse>>(HttpStatusCode.Forbidden, "Forbidden", "ERR_FORBIDDEN", []), cancellation: ct);
                return;
            }

            var users = await context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new UserResponse
                {
                    Id = u.Id.ToString(),
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    MiddleName = u.MiddleName,
                    ProfileURL = u.ProfileURL,
                    RoleId = u.RoleId,
                    IsActive = !u.IsDeleted,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync(ct);

            await SendAsync(new HttpResponse<List<UserResponse>>(users, "Users retrieved successfully.", HttpStatusCode.OK), cancellation: ct);
        }
    }
}
