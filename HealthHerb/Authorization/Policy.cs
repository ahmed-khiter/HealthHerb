using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Authorization
{
    public static class Policy
    {
        public const string PowerAdmin = "PowerAdmin";

        public static AuthorizationPolicy PowerAdminPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireRole(Role.Admin).Build();
        }
    }
}
