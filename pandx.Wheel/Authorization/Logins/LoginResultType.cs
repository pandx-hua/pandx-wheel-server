namespace pandx.Wheel.Authorization.Logins;

public enum LoginResultType
{
    Success,
    InvalidUserNameOrEmail,
    InvalidPassword,
    RequiresTwoFactor,
    UserIsNotActive,
    UserIsLockedOut
}