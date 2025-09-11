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

public sealed record AppError(string Message);

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public ResultTypeEnum Type { get; set; }
    public T? Value { get; }
    public int ValueCount => Value switch
    {
        null => 0,
        ICollection c => c.Count,
        IEnumerable<object> e => e.Count(), 
        _ => 1
    };
    public int TotalCount { get; set; }

    public AppError? Error { get; }

    private Result(bool ok, ResultTypeEnum type, int totalCount = 0, T? value = default, AppError? error = null)
    {
        IsSuccess = ok;
        Type = type;
        TotalCount = totalCount;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value, ResultTypeEnum type, int totalCount = 1) => new(true, type, totalCount, value);
    public static Result<T> Success(ResultTypeEnum type) => new(true, type);
    public static Result<T> Fail(string message, ResultTypeEnum type) => new(false, type, error: new AppError(message));
}
