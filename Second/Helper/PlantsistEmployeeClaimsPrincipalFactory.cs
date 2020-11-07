using First.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace First.Helper
{
    public class PlantsistEmployeeClaimsPrincipalFactory : UserClaimsPrincipalFactory<PlantsistEmployee>
    {
        public PlantsistEmployeeClaimsPrincipalFactory(
            UserManager<PlantsistEmployee> userManager, 
            IOptions<IdentityOptions> optionsAccessor)
            :base(userManager, optionsAccessor)
        {}

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(PlantsistEmployee user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("department", user.Department));
            identity.AddClaim(new Claim("level", user.Level.ToString()));

            return identity;
        }
    }
}
