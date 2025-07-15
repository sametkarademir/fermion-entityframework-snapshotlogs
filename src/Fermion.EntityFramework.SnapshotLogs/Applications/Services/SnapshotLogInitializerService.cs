using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Fermion.Domain.Shared.Interfaces;
using Fermion.EntityFramework.SnapshotLogs.Domain.Entities;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Services;
using Fermion.EntityFramework.SnapshotLogs.Domain.Options;
using Fermion.Domain.Extensions.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fermion.EntityFramework.SnapshotLogs.Applications.Services;

public class SnapshotLogInitializerService(
    ISnapshotLogRepository snapshotLogRepository,
    ISnapshotAssemblyRepository snapshotAssemblyRepository,
    ISnapshotAppSettingRepository snapshotAppSettingRepository,
    IConfiguration configuration,
    IMemoryCache memoryCache,
    IOptions<SnapshotOptions> options,
    ILogger<SnapshotLogInitializerService> logger)
    : ISnapshotLogInitializerService
{
    public async Task TakeSnapshotLogAsync()
    {
        try
        {
            if (!options.Value.Enabled)
            {
                logger.LogInformation("Snapshot logging is disabled. Skipping snapshot initialization.");
                return;
            }

            logger.LogInformation("Starting application snapshot initialization...");
            await ProgressSnapshotLogAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while taking the application snapshot.");
        }
        finally
        {
            logger.LogInformation("Application snapshot initialization completed.");
        }
    }

    private async Task ProgressSnapshotLogAsync()
    {
        var newSnapshotLog = await CreateSnapshotLogAsync();
        memoryCache.Set(nameof(IEntitySnapshotId), newSnapshotLog.Id);
        logger.LogInformation("Snapshot log with ID {SnapshotLogId} has been created and cached.", newSnapshotLog.Id);

        if (options.Value.IsSnapshotAssemblyEnabled)
        {
            await CreateSnapshotAssembliesAsync(newSnapshotLog.Id);
            logger.LogInformation("Snapshot assemblies have been created for SnapshotLog ID {SnapshotLogId}.", newSnapshotLog.Id);
        }

        if (options.Value.IsSnapshotAppSettingEnabled)
        {
            await CreateSnapshotAppSettingsAsync(newSnapshotLog.Id);
            logger.LogInformation("Snapshot app settings have been created for SnapshotLog ID {SnapshotLogId}.", newSnapshotLog.Id);
        }
    }

    #region SnapshotLog

    private async Task<SnapshotLog> CreateSnapshotLogAsync()
    {
        var diskSpaceInfo = GetDiskSpaceInfo();
        var snapshotLog = new SnapshotLog
        {
            ApplicationName = AppDomain.CurrentDomain.FriendlyName,
            ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            MachineName = Environment.MachineName,
            MachineOsVersion = Environment.OSVersion.ToString(),
            Platform = Environment.OSVersion.Platform.ToString(),
            CultureInfo = CultureInfo.CurrentCulture.Name,
            CpuCoreCount = Environment.ProcessorCount.ToString(),
            CpuArchitecture = RuntimeInformation.OSArchitecture.ToString(),
            Hostname = Dns.GetHostName(),
            TotalDiskSpace = diskSpaceInfo.TotalSpace,
            FreeDiskSpace = diskSpaceInfo.FreeSpace,
            IpAddress = GetIpAddress(),
            TotalRam = GetTotalRam()
        };

        snapshotLog = await snapshotLogRepository.AddAsync(snapshotLog);
        await snapshotLogRepository.SaveChangesAsync();

        logger.LogInformation("Snapshot log created with ID: {SnapshotLogId}", snapshotLog.Id);

        return snapshotLog;
    }

    private (string TotalSpace, string FreeSpace) GetDiskSpaceInfo()
    {
        try
        {
            var drives = DriveInfo.GetDrives().Where(d => d is { IsReady: true, DriveType: DriveType.Fixed }).ToList();
            var result = drives
                .GroupBy(item => item.TotalSize)
                .Select(item => item.FirstOrDefault())
                .ToList();

            var totalSpace = result.Sum(item => item?.TotalSize);
            var freeSpace = result.Sum(item => item?.AvailableFreeSpace);

            var totalSpaceFormatted = totalSpace.HasValue ? (totalSpace.Value / (1024 * 1024 * 1024)).ToString("N0") + " GB" : "Unknown";
            var freeSpaceFormatted = freeSpace.HasValue ? (freeSpace.Value / (1024 * 1024 * 1024)).ToString("N0") + " GB" : "Unknown";
            return (totalSpaceFormatted, freeSpaceFormatted);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting total disk space");

            return ("Unknown", "Unknown");
        }
    }

    private string GetIpAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipList = host.AddressList
                .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                .Select(ip => ip.ToString())
                .ToList();

            return string.Join(", ", ipList);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting IP address");

            return "Unknown";
        }
    }

    private string GetTotalRam()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetTotalRamForWindows();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return GetTotalRamForUnix();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetTotalRamFromProcMemInfo();
            }

            return "Unsupported OS";
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting total RAM");

            return "Unknown";
        }
    }

    [SupportedOSPlatform("windows")]
    private string GetTotalRamForWindows()
    {
        try
        {
            const string queryString = "SELECT Capacity FROM Win32_PhysicalMemory";
            using var searcher = new System.Management.ManagementObjectSearcher(queryString);
            var totalBytes = searcher.Get()
                .Cast<System.Management.ManagementObject>()
                .Sum(mo => Convert.ToInt64(mo["Capacity"]));

            return (totalBytes / (1024 * 1024 * 1024)) + " GB";
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error retrieving total RAM on Windows");

            return "Unknown";
        }
    }

    [SupportedOSPlatform("osx")]
    private string GetTotalRamForUnix()
    {
        try
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "sh",
                    Arguments = "-c \"sysctl -n hw.memsize\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(result) && long.TryParse(result.Trim(), out long memBytes))
            {
                return (memBytes / (1024 * 1024 * 1024)) + " GB";
            }

            return "Unknown";
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error retrieving total RAM on Unix");

            return "Unknown";
        }
    }

    [SupportedOSPlatform("linux")]
    private string GetTotalRamFromProcMemInfo()
    {
        try
        {
            var lines = File.ReadAllLines("/proc/meminfo");
            var memTotalLine = lines.FirstOrDefault(line => line.StartsWith("MemTotal:"));

            if (memTotalLine == null)
            {
                return "Unknown";
            }

            var parts = memTotalLine.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && long.TryParse(parts[1], out long memTotalKb))
            {
                return (memTotalKb / (1024 * 1024)) + " GB";
            }

            return "Unknown";
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error retrieving total RAM from /proc/meminfo");

            return "Unknown";
        }
    }

    #endregion

    #region Snapshot Assembly

    private async Task CreateSnapshotAssembliesAsync(Guid snapshotLogId)
    {
        try
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var snapshotAssemblies = assemblies
                .Select(assembly => new SnapshotAssembly
                {
                    Name = assembly.GetName().Name,
                    Version = assembly.GetName().Version?.ToString(),
                    Culture = assembly.GetName().CultureName ?? "neutral",
                    PublicKeyToken = BitConverter.ToString(assembly.GetName().GetPublicKeyToken() ?? []),
                    Location = assembly.Location,
                    SnapshotLogId = snapshotLogId,
                })
                .ToList();

            await snapshotAssemblyRepository.AddRangeAsync(snapshotAssemblies);
            await snapshotAssemblyRepository.SaveChangesAsync();

            logger.LogInformation("Snapshot assemblies created: {AssemblyCount}", snapshotAssemblies.Count);

        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while creating snapshot assemblies.");
        }
    }

    #endregion

    #region Snapshot App Settings

    private async Task CreateSnapshotAppSettingsAsync(Guid snapshotLogId)
    {
        try
        {
            var settings = new Dictionary<string, string>();
            FlattenConfiguration(configuration.AsEnumerable().ToDictionary(k => k.Key, v => v.Value), settings);

            var configJson = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });

            var maskedConfigJson = JsonMaskExtensions.MaskSensitiveData(
                configJson,
                options.Value.MaskPattern,
                options.Value.SensitiveProperties.ToArray());

            if (string.IsNullOrEmpty(maskedConfigJson))
            {
                logger.LogWarning("Masked configuration JSON is empty. No app settings will be created.");

                return;
            }

            var maskedSettings = JsonSerializer.Deserialize<Dictionary<string, string>>(maskedConfigJson) ?? new Dictionary<string, string>();

            var newSnapshotAppSettings = maskedSettings.Select(kvp => new SnapshotAppSetting
            {
                Key = kvp.Key,
                Value = kvp.Value,
                SnapshotLogId = snapshotLogId
            }).ToList();

            await snapshotAppSettingRepository.AddRangeAsync(newSnapshotAppSettings);
            await snapshotAppSettingRepository.SaveChangesAsync();

            logger.LogInformation("Snapshot app settings created for SnapshotLog ID {SnapshotLogId}.", snapshotLogId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while creating snapshot app settings.");
        }
    }

    private void FlattenConfiguration(Dictionary<string, string?> source, Dictionary<string, string> target, string parentKey = "")
    {
        foreach (var kvp in source)
        {
            var fullKey = string.IsNullOrEmpty(parentKey) ? kvp.Key : $"{parentKey}:{kvp.Key}";
            if (!string.IsNullOrEmpty(fullKey))
            {
                target[fullKey] = kvp.Value ?? "null";
            }
        }
    }

    #endregion
}