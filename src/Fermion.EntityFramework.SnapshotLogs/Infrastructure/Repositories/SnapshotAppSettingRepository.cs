using Fermion.EntityFramework.Shared.Repositories;
using Fermion.EntityFramework.SnapshotLogs.Domain.Entities;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.SnapshotLogs.Infrastructure.Repositories;

public class SnapshotAppSettingRepository<TContext> : EfRepositoryBase<SnapshotAppSetting, Guid, TContext>, ISnapshotAppSettingRepository where TContext : DbContext
{
    public SnapshotAppSettingRepository(TContext dbContext) : base(dbContext)
    {
    }
}