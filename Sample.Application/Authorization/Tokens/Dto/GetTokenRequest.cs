using pandx.Wheel.Validation;

namespace Sample.Application.Authorization.Tokens.Dto;

public class GetTokenRequest
{
    public GetTokenRequest(string userNameOrEmail, string password)
    {
        UserNameOrEmail = userNameOrEmail;
        Password = password;
    }

    public string UserNameOrEmail { get; set; }
    public string Password { get; set; }
    public bool IsPersistent { get; set; }
}

public class GetTokenRequestValidator : CustomValidator<GetTokenRequest>
{
}