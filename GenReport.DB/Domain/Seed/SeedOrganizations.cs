namespace GenReport.DB.Domain.Seed
{
    using GenReport.Domain.Entities.Business;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="ApplicationDBContextSeeder" />
    /// </summary>
    public partial class ApplicationDBContextSeeder
    {
        /// <summary>
        /// The SeedOrganizations
        /// </summary>
        public async Task SeedOrganizations()
        {
            var organizations = Enumerable.Range(0, 10).Select(x => new Organization(name :Faker.Company.Name(),Faker.Identification.DateOfBirth(), Faker.Identification.DateOfBirth()));

             await applicationDbContext.Organizations.AddRangeAsync(organizations);
             await applicationDbContext.SaveChangesAsync();
            
        }
    }
}
