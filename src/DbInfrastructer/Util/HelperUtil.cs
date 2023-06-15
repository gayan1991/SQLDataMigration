using Db.Infrastructure.DatabaseServices.Interface;
using Db.Infrastructure.Extensions;
using Db.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Db.Infrastructure.Util
{
    public static class HelperUtil
    {
        private static IMigrationLogger _logger = null!;

        internal static void SetUpLogger(IMigrationLogger logger)
        {
            _logger = logger;
        }

        public static void UpdateForeignTables(MigrationDictionary dict, IEntityType type, bool includeMigTypes = false)
        {
            if (!includeMigTypes && (type == null || type.IsMigrationConfigType()))
            {
                return;
            }

            if (dict.Contains(type))
                dict[type].ReferenceImportanceCount++;
            else
                dict.Add(type);

            var fks = type.GetDeclaredReferencingForeignKeys();

            foreach (var key in type.GetKeys())
            {
                foreach (var property in key.Properties)
                {
                    if (property.ValueGenerated == ValueGenerated.OnAdd && property.GetValueGenerationStrategy() == SqlServerValueGenerationStrategy.IdentityColumn)
                    {
                        dict[type].IsIdentityColumn = true;
                    }
                }
            }

            foreach (var fk in fks)
            {
                if (fk.DeclaringEntityType == fk.PrincipalEntityType)
                {
                    continue;
                }

                UpdateForeignTables(dict, fk.DeclaringEntityType);
            }
        }

        public static void WriteToConsole(string message)
        {
            _logger?.WriteLine(message);
        }
    }
}
