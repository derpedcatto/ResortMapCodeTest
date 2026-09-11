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

        var problemDetail = ErrorCode.InternalError.ToProblemDetails();
        httpContext.Response.StatusCode = problemDetail.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetail, cancellationToken);

        return true;
    }
}