using pandx.Wheel.Events;
using Quartz;

namespace pandx.Wheel.BackgroundJobs;

public class JobExecutionVetoedEvent : Event
{
    public JobExecutionVetoedEvent(IJobExecutionContext context)
    {
        Context = context;
    }

    public IJobExecutionContext Context { get; set; }
}