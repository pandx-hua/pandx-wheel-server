using pandx.Wheel.Authorization;

namespace Sample.Domain;

public static class SampleConsts
{
    public const int DefaultPageSize = 10;

    public static class Role
    {
        public const string Name = RoleNames.Admin;
        public const bool IsStatic = true;
    }

    public static class User
    {
        public const string UserName = UserNames.Admin;
        public const string Password = "good!@#";
        public const string Email = "admin@pandx.com.cn";
    }
}