using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auto_Insurance_System.Filters
{
    public class AutoInsuranceExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            Exception ex = context.Exception;
            context.ExceptionHandled = true;
            string errorMessage = "Server Error " + ex.Message;
            context.HttpContext.Session.SetString("Error", errorMessage);
            context.Result = new ViewResult
            {
                ViewName = "Error"
            };
        }
    }
}
