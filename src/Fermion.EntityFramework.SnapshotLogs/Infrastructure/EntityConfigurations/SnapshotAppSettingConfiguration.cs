using Fermion.EntityFramework.Shared.Extensions;
using Fermion.EntityFramework.SnapshotLogs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fermion.EntityFramework.SnapshotLogs.Infrastructure.EntityConfigurations;

public class SnapshotAppSettingConfiguration : IEntityTypeConfiguration<SnapshotAppSetting>
{
    public void Configure(EntityTypeBuilder<SnapshotAppSetting> builder)
    {
        builder.ApplyGlobalEntityConfigurations();

        builder.ToTable("SnapshotAppSettings");
        builder.HasIndex(item => item.Key);
        builder.HasIndex(item => item.Value);

        builder.Property(item => item.Key).HasMaxLength(256).IsRequired();
        builder.Property(item => item.Value).HasMaxLength(1024).IsRequired();

        builder.HasOne(item => item.SnapshotLog)
            .WithMany(item => item.SnapshotAppSettings)
            .HasForeignKey(item => item.SnapshotLogId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}