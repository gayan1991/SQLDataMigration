using Db.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Db.Infrastructure.Util
{
    public class DBFactory
    {
        public static async Task<T> ConnectAsync<T>(ConnectionModel connectionModel, bool migrate = false) where T : BaseContext<T>
        {
            ProgressIndicator.Show();

            try
            {
                var factory = new DBFactory(connectionModel);
                var context = factory.CreateDbContext<T>();

                if (migrate)
                {
                    await factory.GenerateDatabaseAsync<T>();
                }

                return context;
            }
            finally
            {
                ProgressIndicator.Hide();
            }
        }

        private readonly ConnectionModel _connection;

        internal DBFactory(ConnectionModel connection)
        {
            _connection = connection;
        }

        internal T CreateDbContext<T>() where T : BaseContext<T>
        {
            var optionsBuilder = new DbContextOptionsBuilder<T>();
            optionsBuilder.UseSqlServer("", sqlServerOptionsAction: sqloptions =>
            {
                sqloptions.MigrationsAssembly(typeof(T).GetTypeInfo().Assembly.GetName().Name!);
            });

            if (Activator.CreateInstance(typeof(T), optionsBuilder.Options, _connection) is not T rtn) { return null!; }

            return rtn;
        }

        internal async Task GenerateDatabaseAsync<T>() where T : BaseContext<T>
        {
            try
            {
                await CreateDbContext<T>().Database.MigrateAsync();
            }
            catch
            {
                await Task.Delay(10000);
                await GenerateDatabaseAsync<T>();
            }
        }
    }
}
