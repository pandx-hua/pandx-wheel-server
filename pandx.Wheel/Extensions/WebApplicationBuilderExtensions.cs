using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using pandx.Wheel.Auditing;
using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.BackgroundJobs;
using pandx.Wheel.Caching;
using pandx.Wheel.Controllers;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Domain.UnitOfWork;
using pandx.Wheel.Exceptions;
using pandx.Wheel.Filters;
using pandx.Wheel.Helpers;
using pandx.Wheel.Logging;
using pandx.Wheel.Modules;
using Quartz;
using StackExchange.Redis;

namespace pandx.Wheel.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureWheel(this WebApplicationBuilder builder)
    {
        var assemblies = AssemblyHelper.GetReferencedAssemblies().ToArray();

        builder.InitializeModules(assemblies);
        //logger
        builder.ConfigureLogger();
        //options
        builder.Services.AddOptions<CacheSettings>()
            .Bind(builder.Configuration.GetSection("CacheSettings"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        builder.Services.AddOptions<AuditingSettings>()
            .Bind(builder.Configuration.GetSection("AuditingSettings"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        //mediaR
        builder.Services.AddMediatR(config => { config.RegisterServicesFromAssemblies(assemblies); });
        //autoMapper
        builder.Services.AddAutoMapper(assemblies);
        //fluentValidation
        builder.Services.AddValidatorsFromAssemblies(assemblies);
        //controllers
        builder.Services.AddControllersWithViews().AddControllersAsServices();
        builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, InjectionControllerActivator>());
        //filter
        builder.Services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<ResultFilter>();
            options.Filters.Add<ExceptionFilter>();
            options.Filters.Add<ValidationFilter>();
            options.Filters.Add<NormalizationFilter>();
            options.Filters.Add<ActionFilter>();
        });
        //authorization
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        //signalR
        builder.Services.AddSignalR();
        //backgroundJobs
        builder.Services.AddQuartz();
        builder.Services.AddTransient<IJobListener, BackgroundJobListener>();
        builder.Services.AddTransient<Func<IRepository<BackgroundJobInfo, Guid>>>(sp =>
            sp.GetRequiredService<IRepository<BackgroundJobInfo, Guid>>);
        builder.Services.AddTransient<Func<IUnitOfWork>>(sp => sp.GetRequiredService<IUnitOfWork>);
        //httpContextAccessor
        builder.Services.AddHttpContextAccessor();
        //exception
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        //cache
        var cacheSettings = builder.Configuration.GetSection("CacheSettings").Get<CacheSettings>();
        if (cacheSettings is null)
        {
            return builder;
        }

        if (cacheSettings.UseDistributedCache)
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheSettings.RedisAddress;
                options.InstanceName = cacheSettings.RedisInstanceName;
                options.ConfigurationOptions = new ConfigurationOptions
                {
                    AbortOnConnectFail = true,
                    EndPoints = { cacheSettings.RedisAddress }
                };
            });
        }
        else
        {
            builder.Services.AddMemoryCache();
        }

        return builder;
    }
}