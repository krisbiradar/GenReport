using FastEndpoints;
using GenReport.DB.Domain.Enums;
using GenReport.Domain.DBContext;
using GenReport.Domain.Entities.Onboarding;
using GenReport.Infrastructure.Models.HttpRequests.Core.Users;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GenReport.Api.Endpoints.Core.Users
{
    public class CreateUser(ApplicationDbContext context, ILogger<CreateUser> logger, ICurrentUserService currentUserService) : Endpoint<CreateUserRequest, HttpResponse<string>>
    {
        public override void Configure()
        {
            Post("/users");
        }

        public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
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

            bool isExistingUser = await context.Users.AnyAsync(u => u.Email == req.Email, ct);
            if (isExistingUser)
            {
                await SendAsync(new HttpResponse<string>(HttpStatusCode.Conflict, "User already exists.", "ERR_USER_EXISTS", []), cancellation: ct);
                return;
            }

            var newUser = new User(
                password: req.Password,
                email: req.Email,
                firstName: req.FirstName,
                lastName: req.LastName,
                middleName: req.MiddleName,
                profileURL: req.ProfileURL ?? string.Empty);

            newUser.RoleId = Enum.IsDefined(typeof(Role), req.RoleId) ? req.RoleId : (int)Role.User;
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.UpdatedAt = DateTime.UtcNow;

            await context.Users.AddAsync(newUser, ct);
            await context.SaveChangesAsync(ct);

            await SendAsync(new HttpResponse<string>("Success", "User created successfully.", HttpStatusCode.OK), cancellation: ct);
        }
    }
}
