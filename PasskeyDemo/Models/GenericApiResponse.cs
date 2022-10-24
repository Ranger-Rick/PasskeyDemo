using PasskeyDemo.Interfaces;

namespace PasskeyDemo.Models;

public class GenericApiResponse : IApiResponse
{
    public bool ExecutedSuccessfully { get; init; }
    public string? Message { get; init; }

    public GenericApiResponse(bool executedSuccessfully = true, string? message = null)
    {
        ExecutedSuccessfully = executedSuccessfully;
        Message = message;
    }
}

public class GenericApiResponse<T> : GenericApiResponse, IApiResponse<T>
{
    public T? Result { get; }

    public GenericApiResponse(T? result, string? message = null, bool success = true)
    {
        ExecutedSuccessfully = success;
        Result = result;
        Message = message;
    }
}