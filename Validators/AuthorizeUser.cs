using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Validators
{
    public class AuthorizeUser : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User.Identity.Name;

            //var routeData = context.HttpContext.GetRouteData(); 
            var namedRouteValue = context.HttpContext.GetRouteValue("id").ToString();

            if (user != namedRouteValue)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
