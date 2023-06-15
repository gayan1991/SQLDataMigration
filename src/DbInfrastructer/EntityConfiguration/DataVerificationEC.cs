using Db.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Db.Infrastructure.EntityConfiguration;

public class DataVerificationEC : IEntityTypeConfiguration<DataVerificationModel>
{
    public void Configure(EntityTypeBuilder<DataVerificationModel> builder)
    {
        builder.ToTable("DataVerification", "migration");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.TableName).IsRequired().HasMaxLength(100);
        builder.HasMany(x => x.Queries).WithOne(x => x.VerificationModel).OnDelete(DeleteBehavior.Cascade);
    }
}