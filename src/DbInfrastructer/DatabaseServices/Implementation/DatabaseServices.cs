using Db.Infrastructure.DatabaseServices.Interface;
using Db.Infrastructure.Extensions;
using Db.Infrastructure.Migrations;
using Db.Infrastructure.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Db.Infrastructure.DatabaseServices.Implementation
{
    public class DatabaseServices : IDatabaseService
    {
        private readonly Dictionary<int, IContextService> keyValuePairs = new();

        public void SetUpLogger(IMigrationLogger logger)
        {
            HelperUtil.SetUpLogger(logger);
        }

        public DatabaseServices AddDbContextService(int dbContextId, IContextService contextService)
        {
            keyValuePairs.Add(dbContextId, contextService);
            return this;
        }

        public async Task ResetDb(int dbContextId, ConnectionModel connection)
        {
            HelperUtil.WriteToConsole($"Reset is started for {dbContextId} database");
            await keyValuePairs[dbContextId].ResetDb(connection);
            HelperUtil.WriteToConsole($"Reset is completed for {dbContextId} database");
        }

        public async Task<List<T>> GetResults<T>(int dbContextId, ConnectionModel connection) where T : class
        {
            HelperUtil.WriteToConsole($"{nameof(GetResults)} is called to get data for {typeof(T).Name}");

            if (typeof(T).IsMigrationConfigType())
            {
                HelperUtil.WriteToConsole($"Invalid Type {typeof(T).Name}");
                return null!;
            }

            var result = await keyValuePairs[dbContextId].GetResults<T>(connection);
            HelperUtil.WriteToConsole($"Retrieved {result.Count} records");
            return result;
        }

        public async Task GetResults<T>(int dbContextId, ConnectionModel connection, ResultModel<T> resultModel) where T : class
        {
            HelperUtil.WriteToConsole($"{nameof(GetResults)} is called to get data for {typeof(T).Name}");
            if (typeof(T).IsMigrationConfigType())
            {
                HelperUtil.WriteToConsole($"Invalid Type {typeof(T).Name}");
                return;
            }

            await keyValuePairs[dbContextId].GetResults(connection, resultModel);
            HelperUtil.WriteToConsole($"Retrieved {resultModel.Count} records");
        }

        //public Task InsertResults<T>(int dbContextId, ConnectionModel connection, List<T> values, MigrationEntityMetadata entityMetadata) where T : class
        //{
        //    return keyValuePairs[dbContextId].InsertResults(connection, values, entityMetadata);
        //}

        public async Task InsertResults<T>(int dbContextId, ConnectionModel connection, ResultModel<T> values, MigrationEntityMetadata entityMetadata) where T : class
        {
            HelperUtil.WriteToConsole($"{nameof(InsertResults)} is called to save data for {typeof(T).Name}");
            if (typeof(T).IsMigrationConfigType())
            {
                HelperUtil.WriteToConsole($"Invalid Type {typeof(T).Name}");
                return;
            }

            HelperUtil.WriteToConsole($"{(values.Count > 0 ? "Saving" : "Saved")} {values.Count} records");
            if (values.Count > 0)
            {
                await keyValuePairs[dbContextId].InsertResults(connection, values, entityMetadata);

                HelperUtil.WriteToConsole($"Cosumed {values.CompleteTask} to transfer {typeof(T).Name}");
                await InsertMigrationData(connection, typeof(T).Name, values.Count);
            }
        }

        public async Task<DbStatus> ConnectDbAsync(int dbContextId, ConnectionModel connection, bool migrate = false, bool seedData = false)
        {
            var obj = new DbStatus();
            using (var context = new BaseContext<DbContext>(connection))
            {
                var exists = ((RelationalDatabaseCreator)context.GetService<IDatabaseCreator>()).Exists();
                obj.Status = exists ? Status.Exist : Status.New;
            }

            HelperUtil.WriteToConsole($"Request is made to to connect to {dbContextId} database");
            await keyValuePairs[dbContextId].ConnectDatabase(connection, migrate, seedData);
            HelperUtil.WriteToConsole($"Connection is made to {dbContextId} database");

            return obj;
        }

        public Task<MigrationDictionary> GetEntities(int dbContextId, ConnectionModel connection)
        {
            HelperUtil.WriteToConsole($"Get {dbContextId} entities");
            return keyValuePairs[dbContextId].GetEntities(connection);
        }

        public async Task<ResultModel<T>> GenerateResultModels<T>(int dbContextId, ConnectionModel connection, MigrationEntityMetadata entityMeta)
        {
            HelperUtil.WriteToConsole($"Result Model creation begins for {typeof(T).Name}");

            if (typeof(T).IsMigrationConfigType())
            {
                HelperUtil.WriteToConsole($"Invalid Type {typeof(T).Name}");
                return null!;
            }

            var result = await keyValuePairs[dbContextId].GenerateResultModels<T>(connection, entityMeta);
            HelperUtil.WriteToConsole($"ResultModel is created for {typeof(T).Name}");
            HelperUtil.WriteToConsole($"Number of verification queries: {result.VerificationQueryModel.Keys.Count}");

            return result;
        }

        public Task RemoveMetaTables(ConnectionModel connection)
        {
            return RemoveMigrationTables(connection);
        }

        #region Private

        private async Task InsertMigrationData(ConnectionModel connection, string tableName, int recordsCount)
        {
            using (var context = new BaseContext<DbContext>(connection))
            {
                context.MigrationData.Add(new Model.MigrationDataModel(tableName));
                await context.SaveChangesAsync();
            }
        }

        private async Task RemoveMigrationTables(ConnectionModel connection)
        {
            using var context = new BaseContext<DbContext>(connection);

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    await context.Database.ExecuteSqlRawAsync("Drop Table [Migration].[DataVerificationQueries]");
                    await context.Database.ExecuteSqlRawAsync("Drop Table [Migration].[DataVerification]");
                    await context.Database.ExecuteSqlRawAsync("Drop Table [Migration].[MigrationData]");
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                }
            }
        }

        #endregion
    }
}