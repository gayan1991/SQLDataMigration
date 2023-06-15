using Db.Infrastructure.DatabaseServices.Implementation;
using Db.Infrastructure.DatabaseServices.Interface;
using Microsoft.Extensions.DependencyInjection;
using Migration.Core.Interface;
using Migration.Test;

namespace Migration.Console
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Db Context
            services.AddDbContext<SampleDbContext>();

            services.AddSingleton<IMigrationLogger, Logger>();
            services.AddSingleton<IMigration, Core.Migration>();
            services.AddSingleton<IContextCredential, ContextCredential>();

            var dSvc = new DatabaseServices();
            // Randomly assigned 1 as dbContextId
            dSvc.AddDbContextService(1, new ContextService());

            services.AddSingleton<IDatabaseService, DatabaseServices>(_ => dSvc);

        }
    }
}
