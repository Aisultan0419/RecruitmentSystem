using Microsoft.AspNetCore.Diagnostics;
namespace RecruitmentSystemAPI.Extensions
{
    public class GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken token)
        {
            logger.LogError($"{exception} Unhandled exception happened");

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(
                new
                {
                    errors = new[] {"Internal server error" }
                }, token);

            return true;
        }
    }
}
