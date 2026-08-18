using Microsoft.AspNetCore.Diagnostics;

namespace ResortMap.Server.Common;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception");

        httpContext.Response.StatusCode = 
            Error.InternalError.ToErrorInfo().HttpStatus;

        await httpContext.Response.WriteAsJsonAsync(
            Error.InternalError.ToApiError(), cancellationToken);

        return true;
    }
}
