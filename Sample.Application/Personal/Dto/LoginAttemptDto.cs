using pandx.Wheel.Authorization.Logins;

namespace Sample.Application.Personal.Dto;

public class LoginAttemptDto
{
    public Guid Id { get; set; }
    public DateTime CreationTime { get; set; }

    public Guid? UserId { get; set; }
    public string UserNameOrEmail { get; set; } = default!;
    public string? ClientIpAddress { get; set; } = default!;
    public string? BrowserInfo { get; set; } = default!;
    public LoginResultType Result { get; set; }
}