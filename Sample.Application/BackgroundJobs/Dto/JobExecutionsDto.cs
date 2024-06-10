namespace Sample.Application.BackgroundJobs.Dto;

public class JobExecutionsDto
{
    public Guid BackgroundJobId { get; set; }
    public string JobName { get; set; } = default!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Duration { get; set; }
    public bool Result { get; set; }
    public string? Exception { get; set; }
}