using pandx.Wheel.Application.Dto;

namespace Sample.Application.BackgroundJobs.Dto;

public class ChangeBackgroundJobStatusRequest : EntityDto<Guid>
{
    public bool Status { get; set; }
}