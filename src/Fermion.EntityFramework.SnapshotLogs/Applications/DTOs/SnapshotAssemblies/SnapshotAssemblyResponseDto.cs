using Fermion.Domain.Shared.Auditing;

namespace Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotAssemblies;

public class SnapshotAssemblyResponseDto : CreationAuditedEntity<Guid>
{
    public string? Name { get; set; }
    public string? Version { get; set; }
    public string? Culture { get; set; }
    public string? PublicKeyToken { get; set; }
    public string? Location { get; set; }

    public Guid SnapshotLogId { get; set; }
}