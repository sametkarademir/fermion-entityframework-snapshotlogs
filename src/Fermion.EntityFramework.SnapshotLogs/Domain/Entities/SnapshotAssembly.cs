using Fermion.Domain.Shared.Auditing;
using Fermion.Domain.Shared.Filters;

namespace Fermion.EntityFramework.SnapshotLogs.Domain.Entities;

[ExcludeFromProcessing]
public class SnapshotAssembly : CreationAuditedEntity<Guid>
{
    public string? Name { get; set; }
    public string? Version { get; set; }
    public string? Culture { get; set; }
    public string? PublicKeyToken { get; set; }
    public string? Location { get; set; }

    public Guid SnapshotLogId { get; set; }
    public SnapshotLog? SnapshotLog { get; set; }

    public SnapshotAssembly() : base(Guid.NewGuid())
    {
        CreationTime = DateTime.UtcNow;
    }
}