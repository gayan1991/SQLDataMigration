using Db.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Db.Infrastructure.EntityConfiguration
{
    public class MigrationDataEC : IEntityTypeConfiguration<MigrationDataModel>
    {
        public void Configure(EntityTypeBuilder<MigrationDataModel> builder)
        {
            builder.ToTable("MigrationData", "migration");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.TableName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.LastUpdatedDateTime).HasDefaultValue(DateTimeOffset.Now);
        }
    }
}
