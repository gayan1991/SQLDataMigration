using Db.Infrastructure.Model;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Db.Infrastructure.Migrations
{
    public class MigrationEntityMetadata
    {
        public Type ClrType => EntityType.ClrType;
        public IEntityType EntityType { get; init; }
        public bool IsIdentityColumn { get; set; }
        public int ReferenceImportanceCount { get; set; }

        public bool IsMigrationConfigType => ClrType == typeof(MigrationDataModel) ||
                                                ClrType == typeof(DataVerificationModel) ||
                                                ClrType == typeof(DataVerficiationQueries) ||
                                                ClrType == typeof(VerificationCount);

        public MigrationEntityMetadata(IEntityType entity)
        {
            EntityType = entity;
            ReferenceImportanceCount = 1;
        }
    }
}
