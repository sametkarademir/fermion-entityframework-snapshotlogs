using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fermion.EntityFramework.SnapshotLogs.DependencyInjection;

public class AppInitializer(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Application is starting...");

        using var scope = serviceProvider.CreateScope();
        var snapshotLogInitializerService = scope.ServiceProvider.GetRequiredService<ISnapshotLogInitializerService>();
        await snapshotLogInitializerService.TakeSnapshotLogAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Application is stopping...");

        await Task.CompletedTask;
    }
}