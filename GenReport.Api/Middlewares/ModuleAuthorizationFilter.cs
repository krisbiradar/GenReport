using FastEndpoints;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace GenReport.Middlewares
{
    public class ModuleAuthorizationFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var path = context.HttpContext.Request.Path.Value ?? string.Empty;

            var currentUserService = context.HttpContext.RequestServices.GetService<ICurrentUserService>();
            // If currentUserService is not registered or not available, simply proceed
            if (currentUserService == null) return await next(context);

            var userId = currentUserService.LoggedInUserId();
            if (userId == 0)
            {
                // If not logged in, typically it is a public endpoint (like /login or /refresh), so skip module validation.
                return await next(context);
            }

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

            // Find all modules that actually have a URL prefix configured
            var allModules = await dbContext.Modules
                .Where(m => !string.IsNullOrEmpty(m.UrlPrefix))
                .ToListAsync();

            // Match against the longest URL prefix first to ensure more specific routes match correctly
            var matchedModule = allModules
                .OrderByDescending(m => m.UrlPrefix.Length)
                .FirstOrDefault(m => path.StartsWith(m.UrlPrefix, StringComparison.OrdinalIgnoreCase));

            if (matchedModule == null)
            {
                // If URL does not fall under any module's jurisdiction, let it pass
                return await next(context);
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return Results.Json(new HttpResponse<object>(HttpStatusCode.Unauthorized, "Unauthorized", "ERR_UNAUTHORIZED", []), statusCode: 401);
            }

            // Check if the current user's role has permission for the mapped module
            var hasAccess = await dbContext.RoleModules.AnyAsync(rm => rm.RoleId == user.RoleId && rm.ModuleId == matchedModule.Id);
            if (!hasAccess)
            {
                return Results.Json(new HttpResponse<object>(HttpStatusCode.Forbidden, "Forbidden: Access to this module is denied.", "ERR_FORBIDDEN", []), statusCode: 403);
            }

            return await next(context);
        }
    }
}
