using GenReport.DB.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenReport.DB.Domain.EntityConfigurations
{
    public class MessageReportConfiguration : IEntityTypeConfiguration<MessageReport>
    {
        public void Configure(EntityTypeBuilder<MessageReport> builder)
        {
            builder.HasKey(x => x.Id);

            // MessageReport to Report foreign key (cascade delete)
            builder.HasOne(mr => mr.Report)
                .WithMany()
                .HasForeignKey(mr => mr.ReportId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
