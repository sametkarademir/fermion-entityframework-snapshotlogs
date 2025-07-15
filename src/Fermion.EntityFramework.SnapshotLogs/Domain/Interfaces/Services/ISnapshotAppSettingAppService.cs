using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotAppSettings;

namespace Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Services;

public interface ISnapshotAppSettingAppService
{
    Task<SnapshotAppSettingResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PageableResponseDto<SnapshotAppSettingResponseDto>> GetPageableAndFilterAsync(GetListSnapshotAppSettingRequestDto request, CancellationToken cancellationToken = default);
    Task<int> CleanupOldSnapshotAppSettingAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}