using Db.Infrastructure.DatabaseServices.Implementation;
using Db.Infrastructure.DatabaseServices.Interface;
using Microsoft.Extensions.DependencyInjection;
using Migration.Core.Interface;

namespace Migration.Console
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Confign Db Context
            services.AddDbContext<Config.Infrastructure.ConfigdbContext>();

            // Report Db Context
            services.AddDbContext<Report.Infrastructure.ReportdbContext>();

            // Customer Reference Db Context
            services.AddDbContext<Ref.Infrastructure.RefdbContext>();

            services.AddSingleton<IMigrationLogger, Logger>();
            services.AddSingleton<IMigration, Core.Migration>();
            services.AddSingleton<IContextCredential, ContextCredential>();

            var dSvc = new DatabaseServices();
            dSvc.AddDbContextService(Db.Infrastructure.Enums.DBTypeEnum.ConfigDb, new Config.Infrastructure.ContextService());
            dSvc.AddDbContextService(Db.Infrastructure.Enums.DBTypeEnum.ReportDb, new Report.Infrastructure.ContextService());
            dSvc.AddDbContextService(Db.Infrastructure.Enums.DBTypeEnum.RefDb, new Ref.Infrastructure.ContextService());

            services.AddSingleton<IDatabaseService, DatabaseServices>(_ => dSvc);

        }
    }
}
