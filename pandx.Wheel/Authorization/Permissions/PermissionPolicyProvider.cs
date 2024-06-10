using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace pandx.Wheel.Authorization.Permissions;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        DefaultAuthorizationPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public DefaultAuthorizationPolicyProvider DefaultAuthorizationPolicyProvider { get; }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(WheelClaimTypes.Permission, StringComparison.OrdinalIgnoreCase))
        {
            var policyBuilder = new AuthorizationPolicyBuilder();
            policyBuilder.AddRequirements(new PermissionRequirement(policyName));
            return Task.FromResult<AuthorizationPolicy?>(policyBuilder.Build());
        }

        return DefaultAuthorizationPolicyProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return DefaultAuthorizationPolicyProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return Task.FromResult<AuthorizationPolicy?>(null);
    }
}