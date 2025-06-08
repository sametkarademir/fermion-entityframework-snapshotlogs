using Fermion.EntityFramework.Shared.Repositories.Abstractions;
using Fermion.EntityFramework.SnapshotLogs.Core.Entities;

namespace Fermion.EntityFramework.SnapshotLogs.Core.Interfaces.Repositories;

public interface ISnapshotLogRepository : IRepository<SnapshotLog, Guid>
{
    Task<SnapshotLog?> GetLatestSnapshotLogAsync();
}