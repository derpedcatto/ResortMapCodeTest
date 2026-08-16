namespace ResortMap.Server.Common;

public sealed record Result
{
    public bool IsSuccess { get; }
    public ErrorCode? Error { get; }

    private Result() => IsSuccess = true;
    private Result(ErrorCode error) => (IsSuccess, Error) = (false, error);

    public static Result Success() => new();
    public static Result Failure(ErrorCode error) => new(error);
    
    // usage only in controllers to pass error body without VS editor warnings
    public object ErrorBody => new
    {
        errorCode = Error!.Value.ToString(),
        message = Error!.Value.ToMessage(),
    };
}

public sealed record Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public ErrorCode? Error { get; }

    private Result(T value) => (IsSuccess, Value) = (true, value);
    private Result(ErrorCode error) => (IsSuccess, Error) = (false, error);

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(ErrorCode error) => new(error);

    // usage only in controllers to pass error body without VS editor warnings
    public object ErrorBody => new
    {
        errorCode = Error!.Value.ToString(),
        message = Error!.Value.ToMessage(),
    };
}