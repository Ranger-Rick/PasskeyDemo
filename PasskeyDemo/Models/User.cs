using Fido2NetLib.Development;

namespace PasskeyDemo.Models;

public class User
{
    public byte[] Id { get; init; }
    public string Username { get; init; }
    public string DisplayName { get; init; }
    public string Color { get; init; }

    public StoredCredential Credential { get; init; }
}