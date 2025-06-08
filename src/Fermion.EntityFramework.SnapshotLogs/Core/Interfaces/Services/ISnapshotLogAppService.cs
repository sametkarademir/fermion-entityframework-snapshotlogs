using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotLogs;

namespace Fermion.EntityFramework.SnapshotLogs.Core.Interfaces.Services;

public interface ISnapshotLogAppService
{
    Task<SnapshotLogResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PageableResponseDto<SnapshotLogResponseDto>> GetPageableAndFilterAsync(GetListSnapshotLogRequestDto request, CancellationToken cancellationToken = default);
    Task<int> CleanupOldSnapshotLogsAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}