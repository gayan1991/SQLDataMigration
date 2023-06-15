using Microsoft.Extensions.DependencyInjection;
using Migration.Console;

var host = MigrationHostBuilder.Build(args);

var app = host.Services.GetRequiredService<Main>();
await app.Run();