using Fermion.EntityFramework.Shared.Repositories;
using Fermion.EntityFramework.SnapshotLogs.Domain.Entities;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.SnapshotLogs.Infrastructure.Repositories;

public class SnapshotAssemblyRepository<TContext> : EfRepositoryBase<SnapshotAssembly, Guid, TContext>, ISnapshotAssemblyRepository where TContext : DbContext
{
    public SnapshotAssemblyRepository(TContext dbContext) : base(dbContext)
    {
    }
}