using pandx.Wheel.Application.Dto;

namespace Sample.Application.Auditing.Dto;

public class AuditingDto : EntityDto<Guid>
{
    public Guid UserId { get; set; }

    public string? UserName { get; set; }

    public string? Controller { get; set; }

    public string? Action { get; set; }

    public string? Parameters { get; set; }

    public DateTime ExecutionTime { get; set; }

    public int ExecutionDuration { get; set; }

    public string? ClientIpAddress { get; set; }

    public string? ClientName { get; set; }

    public string? BrowserInfo { get; set; }
    public bool HasException { get; set; }

    public string? Exception { get; set; }
    public string? ReturnValue { get; set; }
}