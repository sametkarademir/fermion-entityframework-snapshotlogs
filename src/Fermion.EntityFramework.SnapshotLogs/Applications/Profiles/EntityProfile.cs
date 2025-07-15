using AutoMapper;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotAppSettings;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotAssemblies;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotLogs;
using Fermion.EntityFramework.SnapshotLogs.Domain.Entities;

namespace Fermion.EntityFramework.SnapshotLogs.Applications.Profiles;

public class EntityProfile : Profile
{
    public EntityProfile()
    {
        CreateMap<SnapshotAppSetting, SnapshotAppSettingResponseDto>();
        CreateMap<SnapshotAssembly, SnapshotAssemblyResponseDto>();
        CreateMap<SnapshotLog, SnapshotLogResponseDto>();
    }
}