using Db.Infrastructure.Enums;
using Db.Infrastructure.Migrations;

namespace Db.Infrastructure.DatabaseServices.Interface
{
    public interface IDatabaseService
    {
        void SetUpLogger(IMigrationLogger logger);

        Task RemoveMetaTables(ConnectionModel connection);

        Task ResetDb(int dbContextId, ConnectionModel connection);

        Task<MigrationDictionary> GetEntities(int dbContextId, ConnectionModel connection);

        Task<ResultModel<T>> GenerateResultModels<T>(int dbContextId, ConnectionModel connection, MigrationEntityMetadata entityMeta);

        Task<DbStatus> ConnectDbAsync(int dbContextId, ConnectionModel connection, bool migrate = false, bool seedData = false);

        Task<List<T>> GetResults<T>(int dbContextId, ConnectionModel connection) where T : class;

        Task GetResults<T>(int dbContextId, ConnectionModel connection, ResultModel<T> resultModel) where T : class;

        //Task InsertResults<T>(int dbContextId, ConnectionModel connection, List<T> values, MigrationEntityMetadata entityMetadata) where T : class;

        Task InsertResults<T>(int dbContextId, ConnectionModel connection, ResultModel<T> values, MigrationEntityMetadata entityMetadata) where T : class;
    }
}
