using AutoMapper;
using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.Shared.Extensions;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotLogs;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fermion.EntityFramework.SnapshotLogs.Applications.Services;

public class SnapshotLogAppService(
    ISnapshotLogRepository snapshotLogRepository,
    IMapper mapper,
    ILogger<SnapshotLogAppService> logger)
    : ISnapshotLogAppService
{
    public async Task<SnapshotLogResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var matchedSnapshotLog = await snapshotLogRepository.GetAsync(
            id: id,
            include: item => item
                .Include(x => x.SnapshotAssemblies)
                .Include(x => x.SnapshotAppSettings),
            enableTracking: false,
            cancellationToken: cancellationToken
        );

        return mapper.Map<SnapshotLogResponseDto>(matchedSnapshotLog);
    }

    public async Task<PageableResponseDto<SnapshotLogResponseDto>> GetPageableAndFilterAsync(GetListSnapshotLogRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = snapshotLogRepository.GetQueryable();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            queryable = queryable
                .Where(item =>
                    (item.ApplicationName != null && item.ApplicationName.Contains(request.Search)) ||
                    (item.ApplicationVersion != null && item.ApplicationVersion.Contains(request.Search)) ||
                    (item.Environment != null && item.Environment.Contains(request.Search)) ||
                    (item.MachineName != null && item.MachineName.Contains(request.Search)) ||
                    (item.Platform != null && item.Platform.Contains(request.Search)) ||
                    (item.IpAddress != null && item.IpAddress.Contains(request.Search)) ||
                    (item.Hostname != null && item.Hostname.Contains(request.Search))
                );
        }

        queryable = queryable.AsNoTracking();
        queryable = queryable.ApplySort(request.Field, request.Order, cancellationToken);
        var result = await queryable.ToPageableAsync(request.Page, request.PerPage, cancellationToken: cancellationToken);
        var matchedSnapshotLogs = mapper.Map<List<SnapshotLogResponseDto>>(result.Data);

        return new PageableResponseDto<SnapshotLogResponseDto>(matchedSnapshotLogs, result.Meta);
    }

    public async Task<int> CleanupOldSnapshotLogAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var queryable = snapshotLogRepository.GetQueryable();
        queryable = queryable.Where(a => a.CreationTime < olderThan);
        var countToDelete = await queryable.CountAsync(cancellationToken);
        if (countToDelete == 0)
        {
            return 0;
        }

        const int batchSize = 100;
        var totalDeleted = 0;
        while (countToDelete > totalDeleted)
        {
            try
            {
                logger.LogInformation(
                    "[CleanupOldSnapshotLogsAsync] [Action=DeleteRangeAsync()] [Count={Count}] [Start]",
                    countToDelete - totalDeleted
                );
                var snapshotLogsToDelete = await queryable
                    .OrderBy(a => a.CreationTime)
                    .Take(batchSize)
                    .ToListAsync(cancellationToken);

                if (snapshotLogsToDelete.Count == 0)
                {
                    logger.LogInformation(
                        "[CleanupOldSnapshotLogsAsync] [Action=DeleteRangeAsync()] [Count={Count}] [NoMoreLogsToDelete]",
                        totalDeleted
                    );

                    break;
                }

                await snapshotLogRepository.DeleteRangeAsync(snapshotLogsToDelete, cancellationToken: cancellationToken);
                await snapshotLogRepository.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "[CleanupOldSnapshotLogsAsync] [Action=DeleteRangeAsync()] [Count={Count}] [End]",
                    snapshotLogsToDelete.Count
                );

                totalDeleted += snapshotLogsToDelete.Count;

                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation(
                        "[CleanupOldSnapshotLogsAsync] [Action=DeleteRangeAsync()] [Cancelled] [TotalDeleted={TotalDeleted}]",
                        totalDeleted
                    );

                    break;
                }

                if (totalDeleted > 0 && totalDeleted % (batchSize * 5) == 0)
                {
                    await Task.Delay(500, cancellationToken);
                }
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "[CleanupOldSnapshotLogsAsync] [Action=DeleteRangeAsync()] [Error] [Exception={Exception}]",
                    e.Message
                );

                break;
            }
        }

        return totalDeleted;
    }
}