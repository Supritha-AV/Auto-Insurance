using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auto_Insurance_System.Filters
{
	public class RoleAuthorizationFilterAttribute : Attribute, IAuthorizationFilter
	{
		private readonly string requiredRole;

		public RoleAuthorizationFilterAttribute(string requiredRole)
		{
			this.requiredRole = requiredRole;
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var sessionRole = context.HttpContext.Session.GetString("UserRole");
			if (string.IsNullOrEmpty(sessionRole) || !string.Equals(sessionRole, requiredRole, StringComparison.OrdinalIgnoreCase))
			{
				context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
			}
		}
	}
} 