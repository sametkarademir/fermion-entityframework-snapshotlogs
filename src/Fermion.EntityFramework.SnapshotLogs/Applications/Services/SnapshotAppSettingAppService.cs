using AutoMapper;
using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.Shared.Extensions;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotAppSettings;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Services;
using Fermion.Domain.Extensions.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fermion.EntityFramework.SnapshotLogs.Applications.Services;

public class SnapshotAppSettingAppService(
    ISnapshotAppSettingRepository snapshotAppSettingRepository,
    IMapper mapper,
    ILogger<SnapshotAppSettingAppService> logger)
    : ISnapshotAppSettingAppService
{
    public async Task<SnapshotAppSettingResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var matchedSnapshotAppSetting = await snapshotAppSettingRepository.GetAsync(
            id: id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );

        return mapper.Map<SnapshotAppSettingResponseDto>(matchedSnapshotAppSetting);
    }

    public async Task<PageableResponseDto<SnapshotAppSettingResponseDto>> GetPageableAndFilterAsync(GetListSnapshotAppSettingRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = snapshotAppSettingRepository.GetQueryable();
        queryable = queryable.WhereIf(request.SnapshotLogId != null, item => item.SnapshotLogId == request.SnapshotLogId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            queryable = queryable
                .Where(item =>
                    item.Key.Contains(request.Search) ||
                    item.Value.Contains(request.Search)
                );
        }

        queryable = queryable.AsNoTracking();
        queryable = queryable.ApplySort(request.Field, request.Order, cancellationToken);
        var result = await queryable.ToPageableAsync(request.Page, request.PerPage, cancellationToken: cancellationToken);
        var matchedSnapshotAppSettings = mapper.Map<List<SnapshotAppSettingResponseDto>>(result.Data);

        return new PageableResponseDto<SnapshotAppSettingResponseDto>(matchedSnapshotAppSettings, result.Meta);
    }

    public async Task<int> CleanupOldSnapshotAppSettingAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var queryable = snapshotAppSettingRepository.GetQueryable();
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
                    "[CleanupOldSnapshotAppSettingsAsync] [Action=DeleteRangeAsync()] [Count={Count}] [Start]",
                    countToDelete - totalDeleted
                );
                var snapshotAppSettingsToDelete = await queryable
                    .OrderBy(a => a.CreationTime)
                    .Take(batchSize)
                    .ToListAsync(cancellationToken);

                if (snapshotAppSettingsToDelete.Count == 0)
                {
                    logger.LogInformation(
                        "[CleanupOldSnapshotAppSettingsAsync] [Action=DeleteRangeAsync()] [Count={Count}] [NoMoreLogsToDelete]",
                        totalDeleted
                    );

                    break;
                }

                await snapshotAppSettingRepository.DeleteRangeAsync(snapshotAppSettingsToDelete, cancellationToken: cancellationToken);
                await snapshotAppSettingRepository.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "[CleanupOldSnapshotAppSettingsAsync] [Action=DeleteRangeAsync()] [Count={Count}] [End]",
                    snapshotAppSettingsToDelete.Count
                );

                totalDeleted += snapshotAppSettingsToDelete.Count;

                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation(
                        "[CleanupOldSnapshotAppSettingsAsync] [Action=DeleteRangeAsync()] [Cancelled] [TotalDeleted={TotalDeleted}]",
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
                    "[CleanupOldSnapshotAppSettingsAsync] [Action=DeleteRangeAsync()] [Error] [Exception={Exception}]",
                    e.Message
                );

                break;
            }
        }

        return totalDeleted;
    }
}