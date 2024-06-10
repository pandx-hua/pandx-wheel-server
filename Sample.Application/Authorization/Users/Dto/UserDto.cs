using pandx.Wheel.Authorization.Users;

namespace Sample.Application.Authorization.Users.Dto;

public class UserDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string? Password { get; set; }
    public Gender Gender { get; set; }
    public bool LockoutEnabled { get; set; }
    public bool IsActive { get; set; }
}