using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Migration.Console
{
    public class MigrationHostBuilder
    {
        private static IHost _hostBuilder;

        internal static IHost MigrationHost => _hostBuilder;

        internal static IHost Build(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureServices(
                    (_, services) =>
                    {
                        var startup = new Startup();
                        startup.ConfigureServices(services);

                        services.AddSingleton<Main, Main>();
                        services.BuildServiceProvider();
                    });     
            
            return _hostBuilder ??= hostBuilder.Build();
        }
    }
}
