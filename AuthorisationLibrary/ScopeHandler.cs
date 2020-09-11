using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthorisationLibrary
{
    public class ScopeHandler : AuthorizationHandler<ScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "scope"))
            {
                return Task.CompletedTask;
            }

            List<Claim> userScopeClaims = context.User.FindAll("scope").ToList();
            foreach (Claim claim in userScopeClaims)
            {
                if (claim.Value.Contains(requirement.ScopeValue))
                {
                    context.Succeed(requirement);
                }                
            }

            return Task.CompletedTask;
        }
    }
}
