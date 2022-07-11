using Microsoft.AspNetCore.Authorization;
using System;

namespace EgtDemo.JWTHelper
{
    public class PermissionRequirement:IAuthorizationRequirement
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }
}
