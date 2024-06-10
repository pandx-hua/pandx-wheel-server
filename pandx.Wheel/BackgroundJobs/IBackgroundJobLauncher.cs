using pandx.Wheel.DependencyInjection;
using Quartz;

namespace pandx.Wheel.BackgroundJobs;

public interface IBackgroundJobLauncher : ITransientDependency
{
    Task StartAsync<TJob>(BackgroundJobData data) where TJob : IJob;
    Task StopAsync(string id);
}