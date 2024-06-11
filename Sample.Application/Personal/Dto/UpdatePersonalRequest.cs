using pandx.Wheel.Storage;
using pandx.Wheel.Validation;

namespace Sample.Application.Personal.Dto;

public class UpdatePersonalRequest:IShouldValidate
{
    public PersonalDto User { get; set; } = default!;
    public CachedFile? CachedFile { get; set; }
}

public class UpdatePersonalRequestValidator : CustomValidator<UpdatePersonalRequest>
{
    public UpdatePersonalRequestValidator()
    {
        
    }
}