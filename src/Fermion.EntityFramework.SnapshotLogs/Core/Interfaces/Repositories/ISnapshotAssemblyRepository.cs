using Fermion.EntityFramework.Shared.Repositories.Abstractions;
using Fermion.EntityFramework.SnapshotLogs.Core.Entities;

namespace Fermion.EntityFramework.SnapshotLogs.Core.Interfaces.Repositories;

public interface ISnapshotAssemblyRepository : IRepository<SnapshotAssembly, Guid>
{
}