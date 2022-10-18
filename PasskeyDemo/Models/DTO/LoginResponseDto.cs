namespace PasskeyDemo.Models.DTO;

public class LoginResponseDto
{
    public bool ExecutedSuccessfully { get; }
    public string UserId { get; init; }
    public string Username { get; init; }
    public string DisplayName { get; init; }
    public string Color { get; init; }
    public string Token { get; init; }

    public LoginResponseDto(bool executedSuccessfully = true)
    {
        ExecutedSuccessfully = executedSuccessfully;
    }
}