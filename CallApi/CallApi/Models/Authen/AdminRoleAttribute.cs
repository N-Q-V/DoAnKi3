using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CallApi.Models.Authen
{
    public class AdminRoleAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var roleClaim = context.HttpContext.Session.GetString("UserRole");

            if (roleClaim != "Admin")
            {
                context.Result = new RedirectToActionResult("Index", "User", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
