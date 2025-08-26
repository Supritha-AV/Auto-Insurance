using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auto_Insurance_System.Filters
{
    public class AdminAuthorizationFilterAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string userId = context.HttpContext.Session.GetString("UserName");
            if (userId != "Admin")
            {
                context.HttpContext.Session.SetString("Error", "You are not authorized to access the given activity or resource");
                context.Result = new ViewResult
                {
                    ViewName = "Error"
                };
            }
        }
    }

}
