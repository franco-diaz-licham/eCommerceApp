namespace Backend.Src.Api.Helpers;

public static class HttpExtensions
{
    /// <summary>
    /// Adds pagination to the HTTP response.
    /// </summary>
    public static void AddPaginationHeader(this HttpResponse response, PaginationMetadata metadata)
    {
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        response.Headers.Append("Pagination", JsonSerializer.Serialize(metadata, options));
        response.Headers.Append(HeaderNames.AccessControlExposeHeaders, "Pagination");
    }

    /// <summary>
    /// Converts service response to appropariate HTTP code response. The exception will be caught by middleware.
    /// </summary>
    public static ActionResult ToActionResult<T>(this Result<T> result, string? locationUrl = null)
    {
        var code = ApiResponse.GetHTTPCode(result.Type);

        if (result.IsSuccess)
        {
            if (result.Value is null) return new NoContentResult();
            var response = new ApiResponse(code, result.Value);

            return code switch
            {
                StatusCodes.Status200OK => new OkObjectResult(response),
                StatusCodes.Status201Created => locationUrl is null ? new ObjectResult(response) { StatusCode = 201 } : new CreatedResult(locationUrl, response),
                StatusCodes.Status202Accepted => new AcceptedResult(locationUrl, response),
                _ => new ObjectResult(response) { StatusCode = code }
            };
        }

        if (result.Error is null) throw new ArgumentException("There is no error to return...");
        var error = new ApiResponse(code, result.Error.Message);

        return code switch
        {
            StatusCodes.Status400BadRequest => new BadRequestObjectResult(error),
            StatusCodes.Status401Unauthorized => new UnauthorizedObjectResult(error),
            StatusCodes.Status403Forbidden => new ObjectResult(error) { StatusCode = 403 },
            StatusCodes.Status404NotFound => new NotFoundObjectResult(error),
            StatusCodes.Status409Conflict => new ConflictObjectResult(error),
            StatusCodes.Status422UnprocessableEntity => new UnprocessableEntityObjectResult(error),
            _ => new ObjectResult(error) { StatusCode = code }
        };
    }
}
