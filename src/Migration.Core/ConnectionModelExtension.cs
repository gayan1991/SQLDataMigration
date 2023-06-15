using Db.Infrastructure.Enums;
using Db.Infrastructure.Migrations;

namespace Migration.Core
{
    public class MigrationConfiguration
    {
        public string SourceServer { get; set; } = null!;
        public string SourceDb { get; set; } = null!;
        public string TargetServer { get; set; } = null!;
        public string TargetDb { get; set; } = null!;
        public int DbContextId { get; set; }
    }

    public static class ConnectionModelExtension
    {
        public static ConnectionModel ToConnectionModel(this MigrationConfiguration configuration, DbEnd dbEnd, string user, string secret)
        {
            return dbEnd switch
            {
                DbEnd.Source =>
                    new ConnectionModel(configuration.SourceServer, user, secret, configuration.SourceDb),
                DbEnd.Target =>
                    new ConnectionModel(configuration.TargetServer, user, secret, configuration.TargetDb),
                _ => throw new NotImplementedException()
            };
        }
    }
}
