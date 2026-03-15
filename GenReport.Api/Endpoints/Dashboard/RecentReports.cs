using FastEndpoints;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.HttpResponse.Dashboard;
using GenReport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GenReport.Endpoints.Dashboard
{
    public class RecentReports(ApplicationDbContext context, ICurrentUserService currentUserService , ILogger<RecentReports> logger) : EndpointWithoutRequest<RecentReportsResponse>
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly ILogger<RecentReports> logger = logger;
        public override void Configure()
        {
            Get("dashboard/reports");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
                
                var userId = _currentUserService.LoggedInUserId();
                var reports = await _context.Reports.Where(x => x.Query.CreatedById == userId).Select(x => new DashboardReportResponse
                {
                    Name = x.Name,
                    RawQuery = x.Query.Rawtext,
                    StorageUrl = x.MediaFile.StorageUrl,
                    CreatedOn = x.CreatedAt,
                    NoOfRows = x.NoOfRows,
                }).ToListAsync(ct);

        }
    }
}
