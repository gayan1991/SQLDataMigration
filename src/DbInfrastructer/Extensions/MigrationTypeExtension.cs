using Db.Infrastructure.Migrations;
using Db.Infrastructure.Model;

namespace Db.Infrastructure.Extensions
{
    public static class MigrationTypeExtension
    {
        public static bool IsMigrationConfigType(this Type type)
        {
            return type == typeof(MigrationDataModel) ||
                   type == typeof(DataVerificationModel) ||
                   type == typeof(DataVerficiationQueries) ||
                   type == typeof(VerificationCount);
        }
    }
}
