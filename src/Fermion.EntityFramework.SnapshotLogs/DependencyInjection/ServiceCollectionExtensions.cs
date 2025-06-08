using System.Reflection;
using Fermion.Domain.Shared.Conventions;
using Fermion.EntityFramework.SnapshotLogs.Applications.Services;
using Fermion.EntityFramework.SnapshotLogs.Core.Interfaces.Repositories;
using Fermion.EntityFramework.SnapshotLogs.Core.Interfaces.Services;
using Fermion.EntityFramework.SnapshotLogs.Core.Options;
using Fermion.EntityFramework.SnapshotLogs.Infrastructure.Repositories;
using Fermion.EntityFramework.SnapshotLogs.Presentation.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fermion.EntityFramework.SnapshotLogs.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFermionSnapshotLogServices<TContext>(this IServiceCollection services, Action<SnapshotOptions> configureOptions) where TContext : DbContext
    {
        var options = new SnapshotOptions();
        configureOptions.Invoke(options);
        services.Configure<SnapshotOptions>(configureOptions.Invoke);

        services.AddScoped<ISnapshotLogRepository, SnapshotLogRepository<TContext>>();
        services.AddScoped<ISnapshotAssemblyRepository, SnapshotAssemblyRepository<TContext>>();
        services.AddScoped<ISnapshotAppSettingRepository, SnapshotAppSettingRepository<TContext>>();

        services.AddScoped<ISnapshotLogInitializerService, SnapshotLogInitializerService>();

        services.AddHostedService<AppInitializer>();
        services.AddMemoryCache();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<ISnapshotLogAppService, SnapshotLogAppService>();
        services.AddScoped<ISnapshotAssemblyAppService, SnapshotAssemblyAppService>();
        services.AddScoped<ISnapshotAppSettingAppService, SnapshotAppSettingAppService>();

        if (options.EnableApiEndpoints)
        {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.ApplicationParts.Add(new AssemblyPart(typeof(SnapshotLogController).Assembly));
                    manager.ApplicationParts.Add(new AssemblyPart(typeof(SnapshotAppSettingController).Assembly));
                    manager.ApplicationParts.Add(new AssemblyPart(typeof(SnapshotAssemblyController).Assembly));
                });

            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerAuthorizationConvention(
                    controllerType: typeof(SnapshotLogController),
                    route: options.ApiRouteSnapshotLog,
                    requireAuthentication: options.Authorization.RequireAuthentication,
                    globalPolicy: options.Authorization.GlobalPolicy,
                    allowedRoles: options.Authorization.EndpointPolicies
                ));
                mvcOptions.Conventions.Add(new ControllerAuthorizationConvention(
                    controllerType: typeof(SnapshotAppSettingController),
                    route: options.ApiRouteSnapshotAppSetting,
                    requireAuthentication: options.Authorization.RequireAuthentication,
                    globalPolicy: options.Authorization.GlobalPolicy,
                    allowedRoles: options.Authorization.EndpointPolicies
                ));
                mvcOptions.Conventions.Add(new ControllerAuthorizationConvention(
                    controllerType: typeof(SnapshotAssemblyController),
                    route: options.ApiRouteSnapshotAssembly,
                    requireAuthentication: options.Authorization.RequireAuthentication,
                    globalPolicy: options.Authorization.GlobalPolicy,
                    allowedRoles: options.Authorization.EndpointPolicies
                ));
            });
        }
        else
        {
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerDisablingConvention(typeof(SnapshotLogController)));
                mvcOptions.Conventions.Add(new ControllerDisablingConvention(typeof(SnapshotAppSettingController)));
                mvcOptions.Conventions.Add(new ControllerDisablingConvention(typeof(SnapshotAssemblyController)));
            });
        }

        return services;
    }
}