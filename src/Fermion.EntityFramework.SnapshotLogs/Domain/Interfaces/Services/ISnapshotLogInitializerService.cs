namespace Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Services;

public interface ISnapshotLogInitializerService
{
    Task TakeSnapshotLogAsync();
}