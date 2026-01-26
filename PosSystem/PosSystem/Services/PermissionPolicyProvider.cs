using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace PosSystem.Services
{
    public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
    {

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // If the policy name starts with our prefix, create a dynamic policy
            if (policyName.StartsWith("Permissions", StringComparison.OrdinalIgnoreCase))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(policyName));
                return policy.Build();
            }

            // Otherwise, use the default behavior
            return await base.GetPolicyAsync(policyName);
        }
    }
}
