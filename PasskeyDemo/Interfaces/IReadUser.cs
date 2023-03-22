using PasskeyDemo.Models;

namespace PasskeyDemo.Interfaces;

public interface IReadUser
{
    Task<User?> GetUser(string username);
    Task<User?> GetUser(byte[] userId);
    Task<bool> IsUsernameAvailable(string username);
}

public interface IReadUserCredential
{
    Task<User?> GetUserByCredentialId(byte[] credentialId);
}