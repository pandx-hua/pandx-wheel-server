using pandx.Wheel.Storage;

namespace Sample.Application.Personal.Dto;

public class UpdatePersonalRequest
{
    public PersonalDto User { get; set; } = default!;
    public CachedFile? CachedFile { get; set; }
}