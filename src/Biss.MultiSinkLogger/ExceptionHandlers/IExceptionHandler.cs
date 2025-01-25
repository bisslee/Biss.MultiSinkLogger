using Microsoft.AspNetCore.Http;

namespace Biss.MultiSinkLogger.ExceptionHandlers
{
    public interface IExceptionHandler
    {
        Task HandleExceptionAsync(HttpContext context, Exception exception);
    }
}
