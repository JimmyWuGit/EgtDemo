using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace EgtDemo.JWTHelper
{
    //public class RequirementHandler : IAuthorizationHandler
    //{
    //    public Task HandleAsync(AuthorizationHandlerContext context)
    //    {
    //        var requirement = context.Requirements.FirstOrDefault();
    //        //context.Succeed(requirement);
    //        context.Fail();

    //        return Task.CompletedTask;
    //    }
    //}

    public class RequirementHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
