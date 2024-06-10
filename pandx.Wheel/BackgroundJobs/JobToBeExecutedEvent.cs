using pandx.Wheel.Events;
using Quartz;

namespace pandx.Wheel.BackgroundJobs;

public class JobToBeExecutedEvent : Event
{
    public JobToBeExecutedEvent(IJobExecutionContext context)
    {
        Context = context;
    }

    public IJobExecutionContext Context { get; set; }
}