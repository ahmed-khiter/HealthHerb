using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Authorization
{
    public static class Policy
    {
        public const string Admin = "Admin";

        public static AuthorizationPolicy AdminPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireRole(Role.Admin).Build();
        }
    }
}
