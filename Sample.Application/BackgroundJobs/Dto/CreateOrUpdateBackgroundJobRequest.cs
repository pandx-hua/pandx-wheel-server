using pandx.Wheel.Validation;

namespace Sample.Application.BackgroundJobs.Dto;

public class CreateOrUpdateBackgroundJobRequest:IShouldValidate
{
    public BackgroundJobDto BackgroundJob { get; set; } = default!;
    public bool IsStartNow { get; set; }
}

public class CreateOrUpdateBackgroundJobRequestValidator : CustomValidator<CreateOrUpdateBackgroundJobRequest>
{
    public CreateOrUpdateBackgroundJobRequestValidator()
    {
        
    }
}