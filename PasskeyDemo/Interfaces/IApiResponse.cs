namespace PasskeyDemo.Interfaces;

/// <summary>
/// Endpoints should always return an implementor of this interface to ensure consistent return values.
/// </summary>
public interface IApiResponse
{
    /// <summary>
    /// A bool which denotes if the API call executed successfully. Clients can check this value before attempting
    /// to access the actual response
    /// </summary>
    bool ExecutedSuccessfully { get; init; }
    
    /// <summary>
    /// A Message that gives more context to the status of the API call
    /// </summary>
    string Message { get; init; }
}

/// <summary>
/// Endpoints should always return an implementor of this interface to ensure consistent return values.
/// </summary>
/// <typeparam name="T">
/// The nullable type of object to return in the <c>Result</c> property. This may be a complex type
/// </typeparam>
public interface IApiResponse<out T> : IApiResponse 
{
    /// <summary>
    /// A nullable object of type <c>T</c> that represents the results of the API call
    /// </summary>
    T? Result { get; }
}