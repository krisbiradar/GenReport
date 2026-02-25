namespace GenReport.Endpoints.Onboarding
{
    using FastEndpoints;
    using GenReport.DB.Domain.Enums;
    using GenReport.Domain.DBContext;
    using GenReport.Infrastructure.Interfaces;
    using GenReport.Infrastructure.Models.HttpRequests.Onboarding;
    using GenReport.Infrastructure.Models.HttpResponse.Onboarding;
    using GenReport.Infrastructure.Models.Shared;
    using GenReport.Infrastructure.Static.Constants;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Defines the <see cref="Login" />
    /// </summary>
    public class Login(ApplicationDbContext context, IApplicationConfiguration configuration, IJWTTokenService jWTTokenService) : Endpoint<LoginRequest, HttpResponse<LoginResponse>>
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IApplicationConfiguration _configuration = configuration;
        private readonly IJWTTokenService jWTTokenService = jWTTokenService;

        public override void Configure()
        {
            Post("/login");
            AllowAnonymous();
        }

        public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == req.Email, cancellationToken: ct);
            if (user == null)
            {
                await SendAsync(new HttpResponse<LoginResponse>(System.Net.HttpStatusCode.Unauthorized, "Please check email", ErrorMessages.USER_NOT_FOUND, [$"user with email {req.Email} not found"]), cancellation: ct);
                return;
            }
            
            if (!user.MatchPassword(req.Password))
            {
                await SendAsync(new HttpResponse<LoginResponse>(System.Net.HttpStatusCode.Unauthorized, "Please check password", ErrorMessages.PASSWORD_DOESNT_MATCH, [$"wrong password for email {req.Email} not found"]), cancellation: ct);
                return;
            }
            
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
            }, $"Hi {user.FirstName} {user.LastName}!", System.Net.HttpStatusCode.OK), cancellation: ct);
        }
    }
}
