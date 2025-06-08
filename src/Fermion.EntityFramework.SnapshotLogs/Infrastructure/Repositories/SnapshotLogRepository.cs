using Fermion.EntityFramework.Shared.Repositories;
using Fermion.EntityFramework.SnapshotLogs.Core.Entities;
using Fermion.EntityFramework.SnapshotLogs.Core.Interfaces.Repositories;
using Fermion.Exceptions.Types;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.SnapshotLogs.Infrastructure.Repositories;

public class SnapshotLogRepository<TContext> : EfRepositoryBase<SnapshotLog, Guid, TContext>, ISnapshotLogRepository where TContext : DbContext
{
    public SnapshotLogRepository(TContext dbContext) : base(dbContext)
    {
    }

    public async Task<SnapshotLog?> GetLatestSnapshotLogAsync()
    {
        var query = Query();
        var latestSnapshot = await query.OrderByDescending(s => s.CreationTime).FirstOrDefaultAsync();

        return latestSnapshot;
    }
}