using Db.Infrastructure.Migrations;
using Db.Infrastructure.Model;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Db.Infrastructure.Extensions
{
    public static class MigrationDictionaryExtension
    {
        public static bool IsMigrationConfigType(this IEntityType entityType)
        {
            return entityType.ClrType == typeof(MigrationDataModel) ||
                   entityType.ClrType == typeof(DataVerificationModel) ||
                   entityType.ClrType == typeof(DataVerficiationQueries) ||
                   entityType.ClrType == typeof(VerificationCount);
        }
    }
}
