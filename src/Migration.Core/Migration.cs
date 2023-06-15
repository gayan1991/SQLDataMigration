using Db.Infrastructure.DatabaseServices.Interface;
using Db.Infrastructure.Enums;
using Db.Infrastructure.Migrations;
using Migration.Core.Interface;
using System.Reflection;

namespace Migration.Core
{
    public class Migration : IMigration
    {
        private readonly IMigrationLogger _logger;
        private readonly IContextCredential _credential;
        private readonly IDatabaseService _databaseService;

        public Migration(IMigrationLogger logger, IContextCredential credential, IDatabaseService databaseService)
        {
            _logger = logger;
            _credential = credential;
            _databaseService = databaseService;
        }

        public async Task MigrateAsync(MigrationConfiguration config)
        {
            _logger.WriteLine($"Request credentials for {config.SourceServer}::{config.SourceDb}");
            var sourceServer = _credential.GetCredential(config, DbEnd.Source);
            _logger.WriteLine("Source server connection model is set");

            _logger.WriteLine($"Request credentials for {config.TargetServer}::{config.TargetDb}");
            var targetServer = _credential.GetCredential(config, DbEnd.Target);
            _logger.WriteLine("Target server connection model is set");

            _databaseService.SetUpLogger(_logger);
            var status = await ConnectDB(targetServer, config.DbContextId, true, true);

            if (status.Status == Status.Exist)
            {
                _logger.WriteLine($"Do you want to reset target {config.TargetServer}.{config.TargetDb} database? Press Y and Enter (Only if reset is needed)", false);
                var response = _logger.ReadLine();

                if (!string.IsNullOrEmpty(response) && response.ToUpper() == "Y")
                {
                    await _databaseService.ResetDb(config.DbContextId, targetServer);
                }
            }

            var completionStatus = new List<(bool, string)>();
            foreach (var entityType in (await _databaseService.GetEntities(config.DbContextId, targetServer)).Values)
            {
                var resultObjTask = ExecuteAsync<Migration>(_databaseService, nameof(IDatabaseService.GenerateResultModels), entityType.ClrType, config.DbContextId, targetServer, entityType);

                if (resultObjTask is null)
                    continue;

                await resultObjTask.ConfigureAwait(false);

                var resultsObjProperty = resultObjTask.GetType().GetProperty("Result");

                if (resultsObjProperty is null)
                    continue;

                var resultObj = resultsObjProperty.GetValue(resultObjTask, null);

                if (resultObj is null)
                    continue;

                var resultsTask = ExecuteAsync<Migration>(_databaseService, nameof(IDatabaseService.GetResults), entityType.ClrType, config.DbContextId, sourceServer, resultObj);

                if (resultsTask is null)
                    continue;

                await resultsTask.ConfigureAwait(false);

                var insertTask = ExecuteAsync<Migration>(_databaseService, nameof(IDatabaseService.InsertResults), entityType.ClrType, config.DbContextId, targetServer, resultObj, entityType);

                if (insertTask is null)
                    continue;

                await insertTask.ConfigureAwait(false);

                if (resultObj.GetType()?.GetProperty("IsSuccessful")?.GetValue(resultObj) is bool successVal)
                {
                    completionStatus.Add(new(successVal, $"Migrations status {(successVal ? "COMPLETED" : "FAILED")} for {entityType.EntityType.Name}"));
                }
            }

            _logger.WriteLine($"Migration is completed {config.SourceServer}::{config.SourceDb} ====>>>> {config.TargetServer}::{config.TargetDb}");

            completionStatus.ForEach(x => _logger.WriteLine(x.Item2));

            if (!completionStatus.All(s => s.Item1))
            {
                _logger.WriteLine("Removing meta tables");
                await _databaseService.RemoveMetaTables(targetServer);
                _logger.WriteLine("Meta tables is removed");
            }
        }

        #region Private

        private Task<DbStatus> ConnectDB(ConnectionModel connection, int dbContextId, bool migrate = false, bool seedData = false)
        {
            return _databaseService.ConnectDbAsync(dbContextId, connection, migrate, seedData);
        }

        private static object? Execute<T>(object obj, string methodName, Type genericType, params object[] parameters)
        {
            var method = typeof(T)
                               .GetMethods().FirstOrDefault(x => x.Name == methodName && x.GetParameters().Length == parameters.Count());

            if (method is null)
                return null;

            var genericMethod = method.MakeGenericMethod(genericType);

            return genericMethod.Invoke(obj, parameters);
        }

        private static Task? ExecuteAsync<T>(object obj, string methodName, Type genericType, params object[] parameters)
        {
            var methods = obj.GetType().IsInterface ? obj.GetType().GetInterfaceMap(genericType).InterfaceMethods : obj.GetType().GetMethods();

            if (methods.FirstOrDefault(x => x.Name == methodName && x.GetParameters().Length == parameters.Length) is not MethodInfo method || method is null)
                return null;

            var genericMethod = method.MakeGenericMethod(genericType);

            return genericMethod.Invoke(obj, parameters) as Task;
        }


        #endregion
    }
}
