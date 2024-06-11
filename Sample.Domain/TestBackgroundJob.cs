using pandx.Wheel.BackgroundJobs;
using Quartz;

namespace Sample.Domain;

[ExposedJob("测试任务")]
public class TestBackgroundJob : BackgroundJob
{
    public override BackgroundJobData BackgroundJobData { get; set; }

    public override Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("TestBackground running.......");
        return Task.CompletedTask;
    }
}