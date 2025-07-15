using Fermion.EntityFramework.SnapshotLogs.Domain.Entities;
using Fermion.EntityFramework.SnapshotLogs.Infrastructure.EntityConfigurations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.SnapshotLogs.Infrastructure.Contexts;

public class SnapshotLogDbContext : DbContext
{
    public DbSet<SnapshotLog> SnapshotLogs { get; set; }
    public DbSet<SnapshotAssembly> SnapshotAssemblies { get; set; }
    public DbSet<SnapshotAppSetting> SnapshotAppSettings { get; set; }

    protected SnapshotLogDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(SnapshotAppSettingConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(SnapshotAssemblyConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(SnapshotLogConfiguration).Assembly);
    }
}