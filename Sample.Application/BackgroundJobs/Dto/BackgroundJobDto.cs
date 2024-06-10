namespace Sample.Application.BackgroundJobs.Dto;

public class BackgroundJobDto
{
    public Guid? Id { get; set; }
    public string JobName { get; set; } = default!;
    public string CronExpression { get; set; } = default!;
    public string? Description { get; set; }
    public string Job { get; set; } = default!;
}