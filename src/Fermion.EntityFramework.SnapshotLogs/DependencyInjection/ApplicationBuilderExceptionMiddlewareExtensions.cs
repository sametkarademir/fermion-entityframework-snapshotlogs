using Microsoft.AspNetCore.Builder;

namespace Fermion.EntityFramework.SnapshotLogs.DependencyInjection;

public static class ApplicationBuilderExceptionMiddlewareExtensions
{
    public static void FermionSnapshotLogMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ProgressingStartedMiddleware>();
    }
}
