namespace Backend.Src.Api.Models;

public class ApiResponse
{
    public ApiResponse(int statusCode, string? message = null)
    {
        StatusCode = statusCode;
        Message = message ?? GetDefaultMessageForStatusCode(statusCode);
    }

    public ApiResponse(int statusCode, object data)
    {
        StatusCode = statusCode;
        Message = GetDefaultMessageForStatusCode(statusCode);
        Data = data;
    }

    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }

    private string? GetDefaultMessageForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "That was a bad request...",
            StatusCodes.Status402PaymentRequired => "Unauthorized...",
            StatusCodes.Status404NotFound => "Resource was not found...",
            StatusCodes.Status500InternalServerError => "There was an internal error...",
            StatusCodes.Status200OK => "Response okay...",
            _ => null
        };
    }

    public static int GetHTTPCode(ResultTypeEnum type)
    {
        return type switch
        {
            ResultTypeEnum.Success => StatusCodes.Status200OK,
            ResultTypeEnum.Accepted => StatusCodes.Status202Accepted,
            ResultTypeEnum.Created => StatusCodes.Status201Created,
            ResultTypeEnum.NotFound => StatusCodes.Status404NotFound,
            ResultTypeEnum.Invalid => StatusCodes.Status400BadRequest,
            ResultTypeEnum.Unauthorized => StatusCodes.Status401Unauthorized,
            ResultTypeEnum.Forbidden => StatusCodes.Status403Forbidden,
            ResultTypeEnum.Conflict => StatusCodes.Status409Conflict,
            ResultTypeEnum.Unprocessable => StatusCodes.Status422UnprocessableEntity,
            ResultTypeEnum.InvalidState => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
