using GenReport.DB.Domain.Entities.Core;
using GenReport.Domain.Entities.Media;
using GenReport.Domain.Entities.Onboarding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenReport.DB.Domain.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<Database> Databases { get; set; }
        public DbSet<Query> Queries { get; set; }
    }
}
