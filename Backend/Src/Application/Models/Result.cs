namespace Backend.Src.Application.Models;

public enum ResultTypeEnum
{
    Success,
    Accepted,
    Created,
    NotFound,
    Invalid,
    Unauthorized,
    Forbidden,
    Conflict,
    Unprocessable,
    InvalidState,
    Unexpected
}

public sealed record AppError(string Code, string Message);

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public ResultTypeEnum Type { get; set; }
    public T? Value { get; }
    public AppError? Error { get; }

    private Result(bool ok, ResultTypeEnum type, T? value, AppError? error)
    {
        IsSuccess = ok;
        Type = type;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value, ResultTypeEnum type) => new(true, type, value, null);
    public static Result<T> Fail(string code, string message, ResultTypeEnum type) => new(false, type, default, new AppError(code, message));
}
