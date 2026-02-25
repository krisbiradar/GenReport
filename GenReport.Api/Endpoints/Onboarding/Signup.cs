using FastEndpoints;
using GenReport.DB.Domain.Enums;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Interfaces;
using GenReport.Infrastructure.Models.HttpRequests.Onboarding;
using GenReport.Infrastructure.Models.HttpResponse.Onboarding;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Infrastructure.Static.Constants;
using Microsoft.EntityFrameworkCore;

namespace GenReport.Endpoints.Onboarding
{
    /// <summary>
    /// Signup endpoint - creates a new user and returns JWT tokens
    /// </summary>
    public class Signup(ApplicationDbContext context, IApplicationConfiguration configuration, IJWTTokenService jWTTokenService) : Endpoint<SignupRequest, HttpResponse<LoginResponse>>
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IApplicationConfiguration _configuration = configuration;
        private readonly IJWTTokenService jWTTokenService = jWTTokenService;

        public override void Configure()
        {
            Post("/signup");
            AllowAnonymous();
        }

        public override async Task HandleAsync(SignupRequest req, CancellationToken ct)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == req.Email, ct);
            if (existingUser != null)
            {
                await SendAsync(new HttpResponse<LoginResponse>(System.Net.HttpStatusCode.Conflict, "Please try using a different email", ErrorMessages.USER_ALREADY_EXISTS, [$"user with email {req.Email} already exists"]), cancellation: ct);
                return;
            }

            var defaultOrganizationId = await _context.Organizations.Select(x => x.Id).FirstOrDefaultAsync(ct);
            var user = new Domain.Entities.Onboarding.User(req.Password, req.Email, req.FirstName, req.LastName, req.MiddleName, defaultOrganizationId, string.Empty);
            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);

            // Generate tokens and return full login response
            var token = jWTTokenService.GenrateAccessToken(user, _configuration.IssuerSigningKey, _configuration.AccessTokenExpiry);
            var refreshToken = jWTTokenService.GenrateAccessToken(user, _configuration.IssuerRefreshKey, _configuration.RefreshTokenExpiry);

            var roleName = Enum.IsDefined(typeof(Role), user.RoleId) ? ((Role)user.RoleId).ToString().ToLower() : "user";

            await SendAsync(new HttpResponse<LoginResponse>(new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                Role = roleName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            }, $"Welcome to GenReport, {user.FirstName}!", System.Net.HttpStatusCode.Created), cancellation: ct);
        }
    }
}
