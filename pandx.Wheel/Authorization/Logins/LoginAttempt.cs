using System.ComponentModel.DataAnnotations;
using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.Authorization.Logins;

public class LoginAttempt:IEntity<Guid>,IHasCreationTime
{
    public Guid Id { get; set; }
    public DateTime CreationTime { get; set; }=DateTime.Now;
    
    public Guid? UserId { get; set; }
    [StringLength(64)]
    public string UserNameOrEmail { get; set; } = default!;
    [StringLength(128)]
    public string? ClientIpAddress { get; set; } 
    [StringLength(512)]
    public string? BrowserInfo { get; set; } 
    public LoginResultType Result { get; set; }
}