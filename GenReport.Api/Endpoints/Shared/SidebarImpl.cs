using FastEndpoints;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.HttpResponse.Shared;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Infrastructure.Static.Constants;
using GenReport.Services.Implementations;
using GenReport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GenReport.Endpoints.Shared
{
    public class SidebarImpl(ILogger<SidebarImpl> logger, ICurrentUserService currentUserService, ApplicationDbContext applicationDbContext) : EndpointWithoutRequest<HttpResponse<List<SidebarImplResponse>>>
    {
        private readonly ILogger<SidebarImpl> _logger = logger;
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

        public override void Configure()
        {
            Get("sidebar/items");
        }
        public override async Task HandleAsync(CancellationToken ct)
        {
            var userId = _currentUserService.LoggedInUserId();
            var user = await _applicationDbContext.Users.FirstOrDefaultAsync(r => r.Id == userId, ct);
            if (user == null)
            {
                _logger.LogError("Invalid access");
                await SendAsync(new HttpResponse<List<SidebarImplResponse>>(System.Net.HttpStatusCode.Unauthorized, "unauthorized access user is not logged in or invalid user id", ErrorMessages.UNAUTHORIZED, []));
            }
            var navItems = await _applicationDbContext.RoleModules.Where(r => r.RoleId == user.RoleId).Select(r => new SidebarImplResponse
            {
                Description = r.Module.Description,
                IconClass = r.Module.IconClass,
                Id = r.Module.Id.ToString(),
                Title = r.Module.Name
            }).ToListAsync(ct);
            await SendAsync(new HttpResponse<List<SidebarImplResponse>>(navItems),200,ct);

        }
    }
}
