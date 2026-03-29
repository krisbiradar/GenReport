using GenReport.DB.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenReport.DB.Domain.EntityConfigurations
{
    public class AiConfigConfiguration : IEntityTypeConfiguration<AiConfig>
    {
        public void Configure(EntityTypeBuilder<AiConfig> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Value)
                .IsRequired()
                .HasColumnType("text");

            builder.HasOne(x => x.AiConnection)
                .WithMany()
                .HasForeignKey(x => x.AiConnectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Nulls in ModelId make unique constraints tricky in Postgres because NULL != NULL.
            // But since Postgres 15+, NULLS NOT DISTINCT makes unique index work if we specify it.
            // Using a filtered unique index when IsActive = true, covering Type, AiConnectionId, and ModelId.
            // To be entirely safe across EF Core versions, we specify an expression index or just 
            // rely on a standard index for the model logic. 
            // Let's create a unique index where IsActive = true for Type, AiConnectionId, ModelId, treating nulls as distinct values.
            builder.HasIndex(x => new { x.Type, x.AiConnectionId, x.ModelId, x.IsActive })
                .HasFilter("\"is_active\" = true AND \"model_id\" IS NOT NULL")
                .IsUnique();

            builder.HasIndex(x => new { x.Type, x.AiConnectionId, x.IsActive })
                .HasFilter("\"is_active\" = true AND \"model_id\" IS NULL")
                .IsUnique();

            builder.HasIndex(x => x.Type);
        }
    }
}
