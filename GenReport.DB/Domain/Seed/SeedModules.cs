using GenReport.DB.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenReport.DB.Domain.Seed
{
    public partial class ApplicationDBContextSeeder
    {
        public async Task SeedModules()
        {
            const long adminRoleId = 2;
            var now = DateTime.UtcNow;
            var requestedModules = new List<Module>
            {
                new Module
                {
                    Name = "Database Connection Management",
                    Description = "Manage and configure database connections",
                    IconClass = "bi bi-database-gear",
                    UrlPrefix = "/connections",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Module
                {
                    Name = "User Management",
                    Description = "Manage users, roles, and account access",
                    IconClass = "bi bi-people",
                    UrlPrefix = "/users",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Module
                {
                    Name = "AI & LLM Configuration",
                    Description = "Configure AI models, prompts, and integration settings",
                    IconClass = "bi bi-cpu",
                    UrlPrefix = "/ai",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Module
                {
                    Name = "Reports",
                    Description = "View and manage generated reports",
                    IconClass = "bi bi-file-earmark-bar-graph",
                    UrlPrefix = "/dashboard/reports",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Module
                {
                    Name = "Chat",
                    Description = "Chat with models and manage sessions",
                    IconClass = "bi bi-chat-dots",
                    UrlPrefix = "/chat",
                    CreatedAt = now,
                    UpdatedAt = now
                }
            };

            var existingModules = await applicationDbContext.Modules.ToListAsync();
            var modulesToMap = new List<Module>();
            bool hasModuleChanges = false;

            foreach (var requestedModule in requestedModules)
            {
                var existingModule = existingModules.FirstOrDefault(m => m.Name.Equals(requestedModule.Name, StringComparison.OrdinalIgnoreCase));
                if (existingModule == null)
                {
                    await applicationDbContext.Modules.AddAsync(requestedModule);
                    modulesToMap.Add(requestedModule);
                    hasModuleChanges = true;
                    continue;
                }

                existingModule.Description = requestedModule.Description;
                existingModule.IconClass = requestedModule.IconClass;
                existingModule.UrlPrefix = requestedModule.UrlPrefix;
                existingModule.UpdatedAt = now;
                modulesToMap.Add(existingModule);
                hasModuleChanges = true;
            }

            if (hasModuleChanges)
            {
                await applicationDbContext.SaveChangesAsync();
            }

            var existingAdminMappings = await applicationDbContext.RoleModules
                .Where(x => x.RoleId == adminRoleId)
                .Select(x => x.ModuleId)
                .ToListAsync();

            var existingModuleIds = existingAdminMappings.ToHashSet();
            bool hasRoleMappingChanges = false;

            foreach (var module in modulesToMap)
            {
                if (existingModuleIds.Contains(module.Id))
                {
                    continue;
                }

                await applicationDbContext.RoleModules.AddAsync(new RoleModuleMapping
                {
                    RoleId = adminRoleId,
                    ModuleId = module.Id
                });
                hasRoleMappingChanges = true;
            }

            if (hasRoleMappingChanges)
            {
                await applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
