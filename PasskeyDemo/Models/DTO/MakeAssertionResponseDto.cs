namespace PasskeyDemo.Models.DTO;

public class MakeAssertionResponseDto
{
    public bool ExecutedSuccessfully { get; init; }
    public string UserId { get; init; }
    public string Username { get; init; }
    public string DisplayName { get; init; }
    public string Color { get; init; }
    public string Token { get; init; }

    public MakeAssertionResponseDto(bool executedSuccessfully = true)
    {
        ExecutedSuccessfully = executedSuccessfully;
    }
}