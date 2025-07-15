using Fermion.EntityFramework.Shared.Interfaces;
using Fermion.EntityFramework.SnapshotLogs.Domain.Entities;

namespace Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Repositories;

public interface ISnapshotAppSettingRepository : IRepository<SnapshotAppSetting, Guid>
{
}