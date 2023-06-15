using Db.Infrastructure.Enums;
using Db.Infrastructure.Migrations;

namespace Db.Infrastructure.DatabaseServices.Interface
{
    public interface IDatabaseService
    {
        void SetUpLogger(IMigrationLogger logger);

        Task RemoveMetaTables(ConnectionModel connection);

        Task ResetDb(DBTypeEnum dBType, ConnectionModel connection);

        Task<MigrationDictionary> GetEntities(DBTypeEnum dBType, ConnectionModel connection);

        Task<ResultModel<T>> GenerateResultModels<T>(DBTypeEnum dBType, ConnectionModel connection, MigrationEntityMetadata entityMeta);

        Task<DbStatus> ConnectDbAsync(DBTypeEnum dBType, ConnectionModel connection, bool migrate = false, bool seedData = false);

        Task<List<T>> GetResults<T>(DBTypeEnum dBType, ConnectionModel connection) where T : class;

        Task GetResults<T>(DBTypeEnum dBType, ConnectionModel connection, ResultModel<T> resultModel) where T : class;

        //Task InsertResults<T>(DBTypeEnum dBType, ConnectionModel connection, List<T> values, MigrationEntityMetadata entityMetadata) where T : class;

        Task InsertResults<T>(DBTypeEnum dBType, ConnectionModel connection, ResultModel<T> values, MigrationEntityMetadata entityMetadata) where T : class;
    }
}
