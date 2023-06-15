using Db.Infrastructure.DatabaseServices.Interface;
using Db.Infrastructure.Extensions;
using Db.Infrastructure.Migrations;
using Db.Infrastructure.Model;
using Db.Infrastructure.Util;
using Microsoft.EntityFrameworkCore;

namespace Migration.Test
{
    public class ContextService : IContextService
    {
        public async Task ConnectDatabase(ConnectionModel connection, bool migrate, bool seedData)
        {
            await DBFactory.ConnectAsync<SampleDbContext>(connection, true);

            if (seedData)
                await SeedData(connection);
        }

        public Task<MigrationDictionary> GetEntities(ConnectionModel connection)
        {
            return GetMigrationDictionary(connection);
        }

        public async Task ResetDb(ConnectionModel connection)
        {
            var data = await GetMigrationDictionary(connection, true);

            foreach (var entity in data.GetValues(false))
            {
                await DeleteData(connection, entity);
            }

            await SeedData(connection);
        }

        public async Task<ResultModel<T>> GenerateResultModels<T>(ConnectionModel connection, MigrationEntityMetadata entityMeta)
        {
            var schema = entityMeta.EntityType.GetSchema();
            var tableName = entityMeta.EntityType.GetTableName();

            if (string.IsNullOrEmpty(tableName))
            {
                return null!;
            }

            var resultObj = new ResultModel<T>
            {
                EntityName = tableName,
                VerificationQueryModel = await GetVerificationQueries(connection, $"{schema ?? "dbo"}.{entityMeta.EntityType.GetTableName()}")
            };

            return resultObj;
        }

        public async Task<List<T>> GetResults<T>(ConnectionModel connection) where T : class
        {
            using (var context = await DBFactory.ConnectAsync<SampleDbContext>(connection, false))
            {
                var rtn = context.EntitySet<T>().ToList();
                return rtn;
            }
        }

        public async Task GetResults<T>(ConnectionModel connection, ResultModel<T> resultModel) where T : class
        {
            if (typeof(T).IsMigrationConfigType())
                return;

            using (var context = await DBFactory.ConnectAsync<SampleDbContext>(connection, false))
            {
                resultModel.Results = await context.EntitySet<T>(resultModel.EntityName).ToListAsync();

                foreach (var key in resultModel.VerificationQueryModel.Keys)
                {
                    var queryString = resultModel.VerificationQueryModel.GetQueury(key);

                    var query = context.VerificationCounts.FromSqlRaw(queryString);

                    resultModel.VerificationQueryModel[key] = (await query.FirstAsync()).RecordCount;
                }
            }
        }

        public async Task InsertResults<T>(ConnectionModel connection, ResultModel<T> obj, MigrationEntityMetadata entityMeta) where T : class
        {
            using (var context = await DBFactory.ConnectAsync<SampleDbContext>(connection))
            {
                await SaveUnderTransaction(context, obj.Results, entityMeta);
                obj.IsSuccessful = await InsertVerificationData<T>(context, obj.VerificationQueryModel);
            }
        }

        public async Task InsertResults<T>(ConnectionModel connection, List<T> obj, MigrationEntityMetadata entityMeta) where T : class
        {
            using var context = await DBFactory.ConnectAsync<SampleDbContext>(connection);
            await SaveUnderTransaction(context, obj, entityMeta);
        }

        #region Private

        private async Task<bool> InsertVerificationData<T>(SampleDbContext context, VerificationQueryModel verificationObj) where T : class
        {
            var isSuccessful = true;
            foreach (var key in verificationObj.Keys)
            {
                var queryString = verificationObj.GetQueury(key);

                var query = context.VerificationCounts.FromSqlRaw(queryString);

                var migratedCount = (await query.FirstAsync()).RecordCount;

                var verificationQuery = await context.DataVerficiationQueries.FindAsync(key);

                if (verificationQuery != null)
                {
                    verificationQuery.SourceCount = verificationObj[key];
                    verificationQuery.DestinationCount = migratedCount;

                    if (isSuccessful && verificationQuery.SourceCount != verificationQuery.DestinationCount)
                    {
                        isSuccessful = !isSuccessful;
                    }
                }

                await context.SaveChangesAsync();
            }

            return isSuccessful;
        }

        private async Task SaveUnderTransaction<T>(SampleDbContext context, List<T> obj, MigrationEntityMetadata entityMeta) where T : class
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                var schema = entityMeta.EntityType.GetSchema();
                var tableName = entityMeta.EntityType.GetTableName();

                if (string.IsNullOrEmpty(tableName))
                {
                    return;
                }

                HelperUtil.WriteToConsole($"Sql Table {tableName}");

                var target = await context.EntitySet<T>(tableName).ToListAsync();

                var pushData = obj.NotExists(target);

                if (pushData == null || pushData.Count == 0)
                {
                    HelperUtil.WriteToConsole("No Data to push target");
                    return;
                }
                else
                    HelperUtil.WriteToConsole($"Pushing target {pushData.Count} record(s)");

                context.EntitySetRange(pushData, tableName);

                if (entityMeta.IsIdentityColumn)
                {
                    IdentityInsertSqlStatement(context, true, tableName, schema);
                }

                await context.SaveChangesAsync();

                HelperUtil.WriteToConsole("Inserted " + typeof(T));
                if (entityMeta.IsIdentityColumn)
                {
                    IdentityInsertSqlStatement(context, false, tableName, schema);
                }

                await transaction.CommitAsync();
            }
        }

        private async Task<VerificationQueryModel> GetVerificationQueries(ConnectionModel connection, string tableName)
        {
            using (var context = await DBFactory.ConnectAsync<SampleDbContext>(connection))
            {
                var dictionary = await context.DataVerficiationQueries
                                            .Include(x => x.VerificationModel)
                                            .Where(x => x.VerificationModel.TableName == tableName).ToDictionaryAsync(x => x.Id, x => x.Query);

                return new VerificationQueryModel().AddQueryDictionary(dictionary);
            }
        }

        /// <summary>
        /// Allow to insert data to a table with Identity data
        /// </summary>
        /// <param name="context">Config Db Context</param>
        /// <param name="tableName">Name of the SQL Table</param>
        /// <param name="identityInsertActivate">Set Identity Insert activation</param>
        private void IdentityInsertSqlStatement(SampleDbContext context, bool identityInsertActivate, string tableName, string? schema = "dbo")
        {
            schema ??= "dbo";
            var setStatement = identityInsertActivate ? "ON" : "OFF";
            context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT [{schema}].[{tableName}] {setStatement}");
            HelperUtil.WriteToConsole($"IDENTITY_INSERT turned {setStatement} for [{schema}].{tableName}");
        }

        private async Task SeedData(ConnectionModel connection)
        {
            var dictionary = await GetEntities(connection);
            if (dictionary != null)
            {
                using (var context = await DBFactory.ConnectAsync<SampleDbContext>(connection))
                {
                    foreach (var type in dictionary.Values)
                    {
                        if (type.IsMigrationConfigType)
                            continue;

                        var schema = type.EntityType.GetSchema();
                        var tableName = type.EntityType.GetTableName();

                        if (await context.DataVerifications.AnyAsync(x => x.TableName == tableName))
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(tableName))
                        {
                            continue;
                        }

                        var newVerificationModel = new DataVerificationModel(tableName, schema!);
                        context.DataVerifications.Add(newVerificationModel);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task DeleteData(ConnectionModel connection, MigrationEntityMetadata entityMeta)
        {
            using var context = await DBFactory.ConnectAsync<SampleDbContext>(connection);

            using (var transaction = context.Database.BeginTransaction())
            {
                var schema = entityMeta.EntityType.GetSchema();
                var tableName = entityMeta.EntityType.GetTableName();

                if (string.IsNullOrEmpty(tableName))
                {
                    return;
                }

                tableName = $"[{schema ?? "dbo"}].[{tableName}]";
                await context.Database.ExecuteSqlRawAsync($"Delete From {tableName}");

                HelperUtil.WriteToConsole($"data deleted from {tableName}");
                await transaction.CommitAsync();
            }
        }

        private async Task<MigrationDictionary> GetMigrationDictionary(ConnectionModel connection, bool includeMigTypes = false)
        {
            using (var context = await DBFactory.ConnectAsync<SampleDbContext>(connection))
            {
                var types = context.Model.GetEntityTypes();
                var dictionary = new MigrationDictionary();

                foreach (var type in types)
                {
                    HelperUtil.UpdateForeignTables(dictionary, type, includeMigTypes);
                }

                return dictionary;
            }
        }

        #endregion
    }
}
