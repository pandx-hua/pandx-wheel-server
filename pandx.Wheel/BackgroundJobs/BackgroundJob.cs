using Quartz;

namespace pandx.Wheel.BackgroundJobs;

public abstract class BackgroundJob : IJob
{
    public abstract BackgroundJobData BackgroundJobData { get; set; }
    public abstract Task Execute(IJobExecutionContext context);
}