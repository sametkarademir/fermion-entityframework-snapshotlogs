using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotLogs;

namespace Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Services;

public interface ISnapshotLogAppService
{
    Task<SnapshotLogResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PageableResponseDto<SnapshotLogResponseDto>> GetPageableAndFilterAsync(GetListSnapshotLogRequestDto request, CancellationToken cancellationToken = default);
    Task<int> CleanupOldSnapshotLogAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}