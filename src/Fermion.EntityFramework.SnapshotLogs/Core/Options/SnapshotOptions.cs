namespace Fermion.EntityFramework.SnapshotLogs.Core.Options;

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

    public bool EnableApiEndpoints { get; set; } = true;
    public string ApiRouteSnapshotLog { get; set; } = "api/snapshot-logs";
    public string ApiRouteSnapshotAssembly { get; set; } = "api/snapshot-assemblies";
    public string ApiRouteSnapshotAppSetting { get; set; } = "api/snapshot-app-settings";
    public AuthorizationOptions Authorization { get; set; } = new();
}

public class AuthorizationOptions
{
    public string? GlobalPolicy { get; set; }
    public bool RequireAuthentication { get; set; } = false;
    public List<string>? EndpointPolicies { get; set; }
}