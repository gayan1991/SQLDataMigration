using Db.Infrastructure.Enums;
using Db.Infrastructure.Migrations;

namespace Migration.Core.Interface
{
    public interface IContextCredential
    {
        public ConnectionModel GetCredential(MigrationConfiguration config, DbEnd db);
    }
}
