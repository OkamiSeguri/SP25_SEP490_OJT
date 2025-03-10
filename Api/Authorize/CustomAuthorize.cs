using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace FOMSOData.Authorize
{
    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {

        private readonly string[] _roles;

        public CustomAuthorize(params string[] roles) 
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new JsonResult(new { code = 401, detail = "Authentication required" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (roleClaim == null || !_roles.Contains(roleClaim))
            {
                context.Result = new JsonResult(new { code = 403, detail = "You do not have permission" })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }
        }
    }
}
