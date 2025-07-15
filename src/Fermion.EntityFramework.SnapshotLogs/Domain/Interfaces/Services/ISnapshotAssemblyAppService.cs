using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotAssemblies;

namespace Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Services;

public interface ISnapshotAssemblyAppService
{
    Task<SnapshotAssemblyResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PageableResponseDto<SnapshotAssemblyResponseDto>> GetPageableAndFilterAsync(GetListSnapshotAssemblyRequestDto request, CancellationToken cancellationToken = default);
    Task<int> CleanupOldSnapshotAssemblyAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}