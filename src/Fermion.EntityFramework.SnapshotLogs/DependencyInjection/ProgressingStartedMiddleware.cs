using Fermion.Domain.Shared.Interfaces;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Repositories;
using Fermion.Domain.Extensions.HttpContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fermion.EntityFramework.SnapshotLogs.DependencyInjection;

public class ProgressingStartedMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ProgressingStartedMiddleware> _logger;

    public ProgressingStartedMiddleware(
        RequestDelegate next,
        IMemoryCache memoryCache,
        ILogger<ProgressingStartedMiddleware> logger)
    {
        _next = next;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            if (!_memoryCache.TryGetValue(nameof(IEntitySnapshotId), out Guid latestSnapshotId))
            {
                var snapshotLogRepository = context.RequestServices.GetRequiredService<ISnapshotLogRepository>();
                var matchedSnapshotLog = await snapshotLogRepository.GetLatestSnapshotLogAsync();
                latestSnapshotId = matchedSnapshotLog?.Id ?? Guid.Empty;
                _memoryCache.Set(nameof(IEntitySnapshotId), latestSnapshotId);
            }

            context.SetSnapshotId(latestSnapshotId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while setting snapshot id");
        }

        await _next(context);
    }
}