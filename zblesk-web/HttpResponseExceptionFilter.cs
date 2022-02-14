using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace zblesk_web;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order { get; } = int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is zblesk_webException exception)
        {
            context.Result = new ObjectResult(exception.Message)
            {
                StatusCode = ((zblesk_webException)context.Exception).StatusCode,
            };
            context.ExceptionHandled = true;
        }
    }
}
