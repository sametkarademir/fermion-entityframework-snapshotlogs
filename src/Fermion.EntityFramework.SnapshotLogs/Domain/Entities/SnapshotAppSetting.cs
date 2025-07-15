using Fermion.Domain.Shared.Auditing;
using Fermion.Domain.Shared.Filters;

namespace Fermion.EntityFramework.SnapshotLogs.Domain.Entities;

[ExcludeFromProcessing]
public class SnapshotAppSetting : CreationAuditedEntity<Guid>
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;

    public Guid SnapshotLogId { get; set; }
    public SnapshotLog? SnapshotLog { get; set; }

    public SnapshotAppSetting() : base(Guid.NewGuid())
    {
        CreationTime = DateTime.UtcNow;
    }
}