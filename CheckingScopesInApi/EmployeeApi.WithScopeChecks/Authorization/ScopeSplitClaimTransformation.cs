using Microsoft.AspNetCore.Authentication;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeApi.WithScopeChecks.Authorization
{
    public class ScopeSplitClaimTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var scopeClaims = principal.FindAll(Claims.ScopeClaimType).ToArray();
            if (scopeClaims.Length != 1 || !scopeClaims[0].Value.Contains(' '))
            {
                // No need to split
                return Task.FromResult(principal);
            }

            var claim = scopeClaims[0];
            var scopes = claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var claims = scopes.Select(s => new Claim(Claims.ScopeClaimType, s));

            return Task.FromResult(new ClaimsPrincipal(new ClaimsIdentity(principal.Identity, claims)));
        }
    }
}
