using AutoMapper;
using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.Shared.Extensions;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotAssemblies;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Services;
using Fermion.Domain.Extensions.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fermion.EntityFramework.SnapshotLogs.Applications.Services;

public class SnapshotAssemblyAppService(
    ISnapshotAssemblyRepository snapshotAssemblyRepository,
    IMapper mapper,
    ILogger<SnapshotAssemblyAppService> logger)
    : ISnapshotAssemblyAppService
{
    public async Task<SnapshotAssemblyResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var matchedSnapshotAssembly = await snapshotAssemblyRepository.GetAsync(
            id: id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );

        return mapper.Map<SnapshotAssemblyResponseDto>(matchedSnapshotAssembly);
    }

    public async Task<PageableResponseDto<SnapshotAssemblyResponseDto>> GetPageableAndFilterAsync(GetListSnapshotAssemblyRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = snapshotAssemblyRepository.GetQueryable();
        queryable = queryable.WhereIf(request.SnapshotLogId != null, item => item.SnapshotLogId == request.SnapshotLogId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            queryable = queryable
                .Where(item =>
                    item.Name != null && item.Name.Contains(request.Search)
                );
        }

        queryable = queryable.AsNoTracking();
        queryable = queryable.ApplySort(request.Field, request.Order, cancellationToken);
        var result = await queryable.ToPageableAsync(request.Page, request.PerPage, cancellationToken: cancellationToken);
        var matchedSnapshotAssemblies = mapper.Map<List<SnapshotAssemblyResponseDto>>(result.Data);

        return new PageableResponseDto<SnapshotAssemblyResponseDto>(matchedSnapshotAssemblies, result.Meta);
    }

    public async Task<int> CleanupOldSnapshotAssemblyAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var queryable = snapshotAssemblyRepository.GetQueryable();
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
                    "[CleanupOldSnapshotAssemblyAsync] [Action=DeleteRangeAsync()] [Count={Count}] [Start]",
                    countToDelete - totalDeleted
                );
                var snapshotAssembliesToDelete = await queryable
                    .OrderBy(a => a.CreationTime)
                    .Take(batchSize)
                    .ToListAsync(cancellationToken);

                if (snapshotAssembliesToDelete.Count == 0)
                {
                    logger.LogInformation(
                        "[CleanupOldSnapshotAssemblyAsync] [Action=DeleteRangeAsync()] [Count={Count}] [NoMoreLogsToDelete]",
                        totalDeleted
                    );

                    break;
                }

                await snapshotAssemblyRepository.DeleteRangeAsync(snapshotAssembliesToDelete, cancellationToken: cancellationToken);
                await snapshotAssemblyRepository.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "[CleanupOldSnapshotAssemblyAsync] [Action=DeleteRangeAsync()] [Count={Count}] [End]",
                    snapshotAssembliesToDelete.Count
                );

                totalDeleted += snapshotAssembliesToDelete.Count;

                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation(
                        "[CleanupOldSnapshotAssemblyAsync] [Action=DeleteRangeAsync()] [Cancelled] [TotalDeleted={TotalDeleted}]",
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
                    "[CleanupOldSnapshotAssemblyAsync] [Action=DeleteRangeAsync()] [Error] [Exception={Exception}]",
                    e.Message
                );

                break;
            }
        }

        return totalDeleted;
    }
}