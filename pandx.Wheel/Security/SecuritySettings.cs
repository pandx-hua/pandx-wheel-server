using System.ComponentModel.DataAnnotations;

namespace pandx.Wheel.Security;

public class JwtBearer
{
    public int AccessTokenExpirationInMinutes { get; set; } = 30;
    public int RefreshTokenExpirationInDays { get; set; } = 30;
    public string Issuer { get; set; } = "pandx.com.cn";
    public string Audience { get; set; } = "pandx.com.cn";
    public string SecurityKey { get; set; } = "Ogt0Tuqq0xPNJex3Bw0QsFYmzausdFFW";
}

public class SecuritySettings : IValidatableObject
{
    public JwtBearer JwtBearer { get; set; } = default!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(JwtBearer.SecurityKey))
        {
            yield return new ValidationResult("没有配置 SecurityKey");
        }
    }
}