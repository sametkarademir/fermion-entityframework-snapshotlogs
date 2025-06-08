using Fermion.Domain.Shared.Auditing;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotAppSettings;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotAssemblies;

namespace Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotLogs;

public class SnapshotLogResponseDto : CreationAuditedEntity<Guid>
{
    public string? ApplicationName { get; set; }
    public string? ApplicationVersion { get; set; }
    public string? Environment { get; set; }
    public string? MachineName { get; set; }
    public string? MachineOsVersion { get; set; }
    public string? Platform { get; set; }
    public string? CultureInfo { get; set; }
    public string? CpuCoreCount { get; set; }
    public string? CpuArchitecture { get; set; }
    public string? TotalRam { get; set; }
    public string? TotalDiskSpace { get; set; }
    public string? FreeDiskSpace { get; set; }
    public string? IpAddress { get; set; }
    public string? Hostname { get; set; }

    public List<SnapshotAssemblyResponseDto> SnapshotAssemblies { get; set; } = [];
    public List<SnapshotAppSettingResponseDto> SnapshotAppSettings { get; set; } = [];
}