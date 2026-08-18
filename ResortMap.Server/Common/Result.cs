using Microsoft.AspNetCore.Mvc;

namespace ResortMap.Server.Common;

public sealed record Result
{
    public bool IsSuccess { get; }
    public Error? Error { get; }

    private Result() => IsSuccess = true;
    private Result(Error error) => (IsSuccess, Error) = (false, error);

    public static Result Success() => new();
    public static Result Failure(Error error) => new(error);
}

public sealed record Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error? Error { get; }

    private Result(T value) => (IsSuccess, Value) = (true, value);
    private Result(Error error) => (IsSuccess, Error) = (false, error);

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
}

public static class ResultExtensions
{
    public static ActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
            return new OkResult();

        var code = result.Error!.Value;
        return new ObjectResult(code.ToApiError()) { StatusCode = code.ToErrorInfo().HttpStatus };
    }

    public static ActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        var code = result.Error!.Value;
        return new ObjectResult(code.ToApiError()) { StatusCode = code.ToErrorInfo().HttpStatus };
    }
}
