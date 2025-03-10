using System.Text.Json.Serialization;

namespace JiuLing.Platform.Models;
public class ApiResponse(int code, string message)
{
    public int Code { get; set; } = code;
    public string Message { get; set; } = message;
}

public class ApiResponse<T>(int code, string message, T? data)
{
    public int Code { get; set; } = code;
    public string Message { get; set; } = message;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; } = data;
}