using Db.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Db.Infrastructure.EntityConfiguration;

public class DataVerificationQueriesEC : IEntityTypeConfiguration<DataVerficiationQueries>
{
    public void Configure(EntityTypeBuilder<DataVerficiationQueries> builder)
    {
        builder.ToTable(nameof(DataVerficiationQueries), "migration");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
    }
}