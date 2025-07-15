using Fermion.Domain.Shared.Conventions;

namespace Fermion.EntityFramework.SnapshotLogs.Domain.Options;

public class SnapshotOptions
{
    public bool Enabled { get; set; } = true;
    public bool IsSnapshotAppSettingEnabled { get; set; } = true;
    public bool IsSnapshotAssemblyEnabled { get; set; } = true;

    public string MaskPattern { get; set; } = "***MASKED***";
    public List<string> SensitiveProperties { get; set; } =
    [
        "Password",
        "Token",
        "Secret",
        "ApiKey",
        "Key",
        "Credential",
        "Ssn",
        "Credit",
        "Card",
        "SecurityCode",
        "Pin",
        "Authorization"
    ];
    
    public SnapshotLogControllerOptions SnapshotLogController { get; set; } = new();
    public SnapshotAssemblyControllerOptions SnapshotAssemblyController { get; set; } = new();
    public SnapshotAppSettingControllerOptions SnapshotAppSettingController { get; set; } = new();
}

public class SnapshotLogControllerOptions
{
    /// <summary>
    /// If true, the SnapshotLogController will be enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Route for the SnapshotLogController
    /// </summary>
    public string Route { get; set; } = "api/snapshot-logs";

    /// <summary>
    /// Authorization settings for SnapshotLog Controller
    /// </summary>
    public AuthorizationOptions GlobalAuthorization { get; set; } = new()
    {
        RequireAuthentication = true,
        Policy = null,
        Roles = null
    };

    /// <summary>
    /// Endpoint-specific authorization settings for SnapshotLog Controller
    /// </summary>
    public List<EndpointOptions>? Endpoints { get; set; }
}

public class SnapshotAssemblyControllerOptions
{
    /// <summary>
    /// If true, the SnapshotAssemblyController will be enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Route for the SnapshotAssemblyController
    /// </summary>
    public string Route { get; set; } = "api/snapshot-assemblies";

    /// <summary>
    /// Authorization settings for SnapshotAssembly Controller
    /// </summary>
    public AuthorizationOptions GlobalAuthorization { get; set; } = new()
    {
        RequireAuthentication = true,
        Policy = null,
        Roles = null
    };

    /// <summary>
    /// Endpoint-specific authorization settings for SnapshotAssembly Controller
    /// </summary>
    public List<EndpointOptions>? Endpoints { get; set; }
}

public class SnapshotAppSettingControllerOptions
{
    /// <summary>
    /// If true, the SnapshotAppSettingController will be enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Route for the SnapshotAppSettingController
    /// </summary>
    public string Route { get; set; } = "api/snapshot-app-settings";

    /// <summary>
    /// Authorization settings for SnapshotAppSetting Controller
    /// </summary>
    public AuthorizationOptions GlobalAuthorization { get; set; } = new()
    {
        RequireAuthentication = true,
        Policy = null,
        Roles = null
    };

    /// <summary>
    /// Endpoint-specific authorization settings for SnapshotAppSetting Controller
    /// </summary>
    public List<EndpointOptions>? Endpoints { get; set; }
}

