using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl.Matchers;

namespace pandx.Wheel.BackgroundJobs;

public class BackgroundJobLauncher : IBackgroundJobLauncher
{
    private readonly IScheduler _scheduler;

    public BackgroundJobLauncher(ISchedulerFactory schedulerFactory, IServiceProvider serviceProvider)
    {
        _scheduler = schedulerFactory.GetScheduler().Result;
        _scheduler.Start();
        var jobListener = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IJobListener>();
        _scheduler.ListenerManager.AddJobListener(jobListener, GroupMatcher<JobKey>.AnyGroup());
    }

    public async Task StartAsync<TJob>(BackgroundJobData data) where TJob : IJob
    {
        var job = JobBuilder.Create<TJob>()
            .WithIdentity(data.Id)
            .SetJobData(new JobDataMap { new("BackgroundJobData", data) })
            .Build();
        var triggerBuilder = TriggerBuilder.Create();
        var trigger = string.IsNullOrWhiteSpace(data.CronExpression)
            ? triggerBuilder.WithIdentity(data.Id)
                .StartNow()
                .WithSimpleSchedule(b => b.WithRepeatCount(0))
                .Build()
            : triggerBuilder.WithIdentity(data.Id)
                .StartNow()
                .WithCronSchedule(data.CronExpression)
                .Build();
        await _scheduler.ScheduleJob(job, trigger);
    }

    public async Task StopAsync(string id)
    {
        var triggerKey = new TriggerKey(id);
        await _scheduler.PauseTrigger(triggerKey);
        await _scheduler.UnscheduleJob(triggerKey);
        await _scheduler.DeleteJob(new JobKey(id));
    }
}