using pandx.Wheel.Application.Dto;
using pandx.Wheel.Authorization.Users;

namespace Sample.Application.Personal.Dto;

public class PersonalDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;

    public string PhoneNumber { get; set; } = default!;
    
    public Gender Gender { get; set; }
}