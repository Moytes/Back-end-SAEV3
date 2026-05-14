namespace Utilities.Responses;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string IntOpCode { get; set; } = string.Empty;
    public T? Data { get; set; }

    public ApiResponse(int statusCode, string intOpCode, T? data)
    {
        StatusCode = statusCode;
        IntOpCode = intOpCode;
        Data = data;
    }
}
