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
            var dbConnectionsModule = await applicationDbContext.Modules.FirstOrDefaultAsync(m => m.Name == "Database Connections");

            if (dbConnectionsModule == null)
            {
                dbConnectionsModule = new Module
                {
                    Name = "Database Connections",
                    Description = "Manage database connections",
                    IconClass = "bi bi-database-add", // Uses a Bootstrap icon for the sidebar
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await applicationDbContext.Modules.AddAsync(dbConnectionsModule);
                await applicationDbContext.SaveChangesAsync();

                // Get all users, and give them access. Or map to roles if a specific Role model is preferred. 
                // For MVP: Map to all distinct role IDs found in the system
                var roles = await applicationDbContext.Users.Select(u => u.RoleId).Distinct().ToListAsync();

                foreach(var roleId in roles)
                {
                    await applicationDbContext.RoleModules.AddAsync(new RoleModuleMapping
                    {
                        RoleId = roleId,
                        ModuleId = dbConnectionsModule.Id
                    });
                }

                await applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
