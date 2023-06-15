using Db.Infrastructure.Enums;
using Db.Infrastructure.Migrations;

namespace Db.Infrastructure.DatabaseServices.Interface
{
    public interface IContextService
    {
        Task ResetDb(ConnectionModel connection);

        Task<ResultModel<T>> GenerateResultModels<T>(ConnectionModel connection, MigrationEntityMetadata entityMeta);

        Task<MigrationDictionary> GetEntities(ConnectionModel connection);

        Task ConnectDatabase(ConnectionModel connection, bool migrate, bool seedData);

        Task<List<T>> GetResults<T>(ConnectionModel connection) where T : class;

        Task GetResults<T>(ConnectionModel connection, ResultModel<T> resultModel) where T : class;

        Task InsertResults<T>(ConnectionModel connection, List<T> obj, MigrationEntityMetadata entityMeta) where T : class;

        Task InsertResults<T>(ConnectionModel connection, ResultModel<T> values, MigrationEntityMetadata entityMetadata) where T : class;
    }
}
