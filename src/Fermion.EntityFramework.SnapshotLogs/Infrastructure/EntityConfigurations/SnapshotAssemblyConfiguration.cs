using Fermion.EntityFramework.Shared.Extensions;
using Fermion.EntityFramework.SnapshotLogs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fermion.EntityFramework.SnapshotLogs.Infrastructure.EntityConfigurations;

public class SnapshotAssemblyConfiguration : IEntityTypeConfiguration<SnapshotAssembly>
{
    public void Configure(EntityTypeBuilder<SnapshotAssembly> builder)
    {
        builder.ApplyGlobalEntityConfigurations();

        builder.ToTable("SnapshotAssemblies");
        builder.HasIndex(item => item.Name);

        builder.Property(item => item.Name).HasMaxLength(256).IsRequired(false);
        builder.Property(item => item.Version).HasMaxLength(64).IsRequired(false);
        builder.Property(item => item.Culture).HasMaxLength(64).IsRequired(false);
        builder.Property(item => item.PublicKeyToken).HasMaxLength(128).IsRequired(false);
        builder.Property(item => item.Location).HasMaxLength(1024).IsRequired(false);

        builder.HasOne(item => item.SnapshotLog)
            .WithMany(item => item.SnapshotAssemblies)
            .HasForeignKey(item => item.SnapshotLogId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}