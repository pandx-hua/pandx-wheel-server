using pandx.Wheel.Events;
using Quartz;

namespace pandx.Wheel.BackgroundJobs;

public class JobWasExecutedEvent : Event
{
    public JobWasExecutedEvent(IJobExecutionContext context, JobExecutionException? jobException)
    {
        Context = context;
        JobException = jobException;
    }

    public IJobExecutionContext Context { get; set; }
    public JobExecutionException? JobException { get; set; }
}