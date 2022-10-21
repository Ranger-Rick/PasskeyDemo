using PasskeyDemo.Models;

namespace PasskeyDemo.Interfaces;

public interface ITokenGenerator
{
    /// <summary>
    /// Implementors of this method can generate an access token that signify if the user is currently logged in
    /// </summary>
    /// <param name="user">The user for which to generate the token</param>
    /// <returns>A string representation of the token</returns>
    string GenerateToken(User user);
}