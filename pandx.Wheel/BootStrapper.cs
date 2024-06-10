using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.BackgroundJobs;
using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Helpers;
using pandx.Wheel.Initializers;
using pandx.Wheel.Menus;
using pandx.Wheel.Notifications;
using Quartz;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace pandx.Wheel;

public static class Bootstrapper
{
    public static void ConfigureBootstrapLogger()
    {
        var outputTemplate =
            "[{Level:u3}] {Timestamp:yyyy-MM-dd HH:mm:ss.fff} | {MachineName} {ProcessName}/{ProcessId} {ThreadId} {SourceContext:l} {Message:lj}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "pandx.Wheel")
            .Enrich.WithProcessName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadName()
            .Enrich.WithThreadId()
            .Enrich.WithMachineName()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console(outputTemplate: outputTemplate)
            .WriteTo.File("Logs/start-exception.txt", outputTemplate: outputTemplate,
                rollingInterval: RollingInterval.Day)
            .CreateBootstrapLogger();
    }

    public static async Task InitializeAsync(WebApplication app)
    {
        Log.Information("********** 初始化应用开始 **********");
        ServiceLocator.Instance = app.Services.CreateScope().ServiceProvider;
        using var scope = app.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IPermissionManager>().InitializeAsync();
        await scope.ServiceProvider.GetRequiredService<INotificationDefinitionManager>().InitializeAsync();
        await scope.ServiceProvider.GetRequiredService<IMenuManager>().InitializeAsync();
        if (scope.ServiceProvider.GetService<IDbInitializer>() is { } dbInitializer)
        {
            await dbInitializer.InitializeAsync();
        }

        if (scope.ServiceProvider.GetService<ICustomInitializer>() is { } customInitializer)
        {
            await customInitializer.InitializeAsync();
        }

        var backgroundJobLauncher = scope.ServiceProvider.GetRequiredService<IBackgroundJobLauncher>();
        var backgroundJobRepository = scope.ServiceProvider.GetRequiredService<IRepository<BackgroundJobInfo, Guid>>();
        var backgroundJobs = await backgroundJobRepository.GetAllListAsync(b => b.Status);
        foreach (var backgroundJob in backgroundJobs)
        {
            var jobType = AssemblyHelper.GetReferencedAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.FullName == backgroundJob.Job))
                .FirstOrDefault();
            if (jobType is null)
            {
                Log.Error("找不到类型 {Job}", backgroundJob.Job);
                continue;
            }

            if (!typeof(IJob).IsAssignableFrom(jobType))
            {
                Log.Error("{Job} 类型没有实现IJob", backgroundJob.Job);
                continue;
            }

            var method = typeof(IBackgroundJobLauncher).GetMethod("StartAsync");
            var genericMethod = method!.MakeGenericMethod(jobType);
            await (Task)genericMethod.Invoke(backgroundJobLauncher, new object[]
            {
                new BackgroundJobData
                {
                    Id = backgroundJob.Id.ToString(),
                    CronExpression = backgroundJob.CronExpression
                }
            })!;
        }

        Log.Information("********** 初始化应用结束 **********");
    }
}