namespace pandx.Wheel.Authorization;

public static class UserIdentifierExtensions
{
    public static UserIdentifier ToUserIdentifier(this IUserIdentifier userIdentifier)
    {
        return new UserIdentifier(userIdentifier.UserId.ToString());
    }
}