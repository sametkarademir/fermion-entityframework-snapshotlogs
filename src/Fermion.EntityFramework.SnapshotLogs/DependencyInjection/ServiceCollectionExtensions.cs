using System.Reflection;
using Fermion.Domain.Shared.Conventions;
using Fermion.EntityFramework.SnapshotLogs.Applications.Services;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Services;
using Fermion.EntityFramework.SnapshotLogs.Domain.Options;
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

        if (options.SnapshotLogController.Enabled)
        {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.ApplicationParts.Add(new AssemblyPart(typeof(SnapshotLogController).Assembly));
                });
            
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerAuthorizationConvention(
                    typeof(SnapshotLogController),
                    options.SnapshotLogController.Route,
                    options.SnapshotLogController.GlobalAuthorization,
                    options.SnapshotLogController.Endpoints
                ));
            });
        }
        else
        {
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerDisablingConvention(typeof(SnapshotLogController)));
            });
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerRemovalConvention(typeof(SnapshotLogController)));
            });
        }
        
        if (options.SnapshotAssemblyController.Enabled)
        {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.ApplicationParts.Add(new AssemblyPart(typeof(SnapshotAssemblyController).Assembly));
                });
            
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerAuthorizationConvention(
                    typeof(SnapshotAssemblyController),
                    options.SnapshotAssemblyController.Route,
                    options.SnapshotAssemblyController.GlobalAuthorization,
                    options.SnapshotAssemblyController.Endpoints
                ));
            });
        }
        else
        {
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerDisablingConvention(typeof(SnapshotAssemblyController)));
            });
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerRemovalConvention(typeof(SnapshotAssemblyController)));
            });
        }
        
        if (options.SnapshotAppSettingController.Enabled)
        {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.ApplicationParts.Add(new AssemblyPart(typeof(SnapshotAppSettingController).Assembly));
                });
            
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerAuthorizationConvention(
                    typeof(SnapshotAppSettingController),
                    options.SnapshotAppSettingController.Route,
                    options.SnapshotAppSettingController.GlobalAuthorization,
                    options.SnapshotAppSettingController.Endpoints
                ));
            });
        }
        else
        {
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerDisablingConvention(typeof(SnapshotAppSettingController)));
            });
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerRemovalConvention(typeof(SnapshotAppSettingController)));
            });
        }

        return services;
    }
}