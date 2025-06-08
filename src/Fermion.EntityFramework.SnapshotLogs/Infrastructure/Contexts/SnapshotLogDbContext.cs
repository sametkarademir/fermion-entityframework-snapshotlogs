using Fermion.EntityFramework.Shared.Extensions;
using Fermion.EntityFramework.SnapshotLogs.Core.Entities;
using Fermion.EntityFramework.SnapshotLogs.Infrastructure.EntityConfigurations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.SnapshotLogs.Infrastructure.Contexts;

public class SnapshotLogDbContext : DbContext
{
    public DbSet<SnapshotLog> SnapshotLogs { get; set; }
    public DbSet<SnapshotAssembly> SnapshotAssemblies { get; set; }
    public DbSet<SnapshotAppSetting> SnapshotAppSettings { get; set; }

    private readonly IHttpContextAccessor _httpContextAccessor;

    protected SnapshotLogDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(SnapshotAppSettingConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(SnapshotAssemblyConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(SnapshotLogConfiguration).Assembly);
    }

    public override int SaveChanges()
    {
        this.SetCreationTimestamps(_httpContextAccessor);
        this.SetModificationTimestamps(_httpContextAccessor);
        this.SetSoftDelete(_httpContextAccessor);
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.SetCreationTimestamps(_httpContextAccessor);
        this.SetModificationTimestamps(_httpContextAccessor);
        this.SetSoftDelete(_httpContextAccessor);
        return await base.SaveChangesAsync(cancellationToken);
    }
}