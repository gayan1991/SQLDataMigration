using Db.Infrastructure.DatabaseServices.Interface;
using Migration.Core;
using Migration.Core.Interface;
using Newtonsoft.Json;

namespace Migration.Console
{
    public class Main
    {
        private const string fileName = "MigrationConfig.json";

        private readonly List<MigrationConfiguration> MigrationConfigurations = null!;

        private readonly IMigration _migration;

        public Main(IMigration migration, IMigrationLogger logger)
        {
            _migration = migration;

            var file = System.IO.File.ReadAllText(fileName);
            logger.WriteLine($"{fileName} is requested");

            if (string.IsNullOrEmpty(file))
            {
                logger.WriteLine($"{fileName} is not found");
                return;
            }

            MigrationConfigurations = JsonConvert.DeserializeObject<List<MigrationConfiguration>>(file)!;
            logger.WriteLine($"Configuration is set from Migration Config");
        }

        public async Task Run()
        {
            foreach (var config in MigrationConfigurations)
            {
                await _migration.MigrateAsync(config);
            }
        }
    }
}
