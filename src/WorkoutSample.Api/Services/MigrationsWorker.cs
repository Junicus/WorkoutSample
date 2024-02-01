using Microsoft.EntityFrameworkCore;
using WorkoutSample.Infrastructure.Persistence;

namespace WorkoutSample.Api.Services;

public class MigrationsWorker(
    IServiceProvider serviceProvider,
    IWebHostEnvironment env,
    ILogger<MigrationsWorker> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (env.IsDevelopment())
        {
            logger.LogInformation("Applying migrations");
            await using var scope = serviceProvider.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<WorkoutDbContext>();
            await context.Database.MigrateAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}