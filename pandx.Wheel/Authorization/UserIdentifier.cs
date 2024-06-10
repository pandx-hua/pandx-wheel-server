namespace pandx.Wheel.Authorization;

[Serializable]
public class UserIdentifier : IUserIdentifier
{
    public UserIdentifier(string userId)
    {
        UserId = Guid.Parse(userId);
    }

    //此处对UserId进行一下封装，方便日后加入多租户功能
    public Guid UserId { get; }

    public override bool Equals(object? obj)
    {
        if (obj is UserIdentifier userIdentifier)
        {
            return userIdentifier.UserId == UserId;
        }

        return false;
    }

    public override int GetHashCode()
    {
        var hash = 19;
        return hash * 23 + UserId.GetHashCode();
    }

    public override string ToString()
    {
        return UserId.ToString();
    }

    public static UserIdentifier Parse(string userIdentifierString)
    {
        return new UserIdentifier(userIdentifierString);
    }
}