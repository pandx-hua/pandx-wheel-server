using pandx.Wheel.Authorization;

namespace pandx.Wheel.Extensions;

public static class GuidExtensions
{
    public static UserIdentifier ToUserIdentifier(this Guid guid)
    {
        return new UserIdentifier(guid.ToString());
    }
}