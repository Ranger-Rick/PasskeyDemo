using PasskeyDemo.Interfaces;

namespace PasskeyDemo.Models;

public class GenericApiResponse<T> : IApiResponse<T>
{
    public bool ExecutedSuccessfully { get; init; }
    public string? Message { get; init; }
    public T? Result { get; }

    public GenericApiResponse(T? result, string? message = null, bool success = true)
    {
        ExecutedSuccessfully = success;
        Result = result;
        Message = message;
    }

    public GenericApiResponse() {}
}