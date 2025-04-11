namespace MediaService.src.WebApi.Controllers.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        if (exception is ArgumentException argumentException)
        {
            context.Result = new BadRequestObjectResult(new { message = argumentException.Message });
        }
        else
        {
            context.Result = new ObjectResult(new { message = exception.Message })
            {
                StatusCode = 500,
            };
        }

        context.ExceptionHandled = true;
    }
}
