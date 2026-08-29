namespace ONEE.EAMS.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data, int statusCode = 200) =>
        new() { Success = true, Data = data, StatusCode = statusCode };

    public static ApiResponse<T> Fail(string message, int statusCode, IEnumerable<string>? details = null) =>
        new() { Success = false, Error = new ApiError { Message = message, Details = details?.ToList() ?? [] }, StatusCode = statusCode };
}

public class ApiError
{
    public string Message { get; set; } = string.Empty;
    public List<string> Details { get; set; } = [];
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
