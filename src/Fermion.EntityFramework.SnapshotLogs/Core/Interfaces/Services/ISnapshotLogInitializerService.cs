namespace Fermion.EntityFramework.SnapshotLogs.Core.Interfaces.Services;

public interface ISnapshotLogInitializerService
{
    Task TakeSnapshotLogAsync();
}