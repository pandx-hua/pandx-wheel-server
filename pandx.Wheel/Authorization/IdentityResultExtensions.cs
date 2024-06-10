using Microsoft.AspNetCore.Identity;

namespace pandx.Wheel.Authorization;

public static class IdentityResultExtensions
{
    public static void CheckErrors(this IdentityResult identityResult)
    {
        if (identityResult.Succeeded)
        {
            return;
        }

        throw new Exception(string.Join(",", identityResult.Errors));
    }
}