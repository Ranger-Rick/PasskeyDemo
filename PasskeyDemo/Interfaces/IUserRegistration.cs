using Fido2NetLib;
using PasskeyDemo.Models;

namespace PasskeyDemo.Interfaces;

public interface IUserRegistration
{
    /// <summary>
    /// Accepts a <c>CredentialMakeResult</c> and <c>CredentialCreateOptions</c> to create/register a new user.
    /// This contract allows for project specific user initialization/registration steps
    /// </summary>
    /// <param name="credential">The result of the <c>Fido2.MakeNewCredentialAsync</c> method</param>
    /// <param name="options">The result of the <c>Fido2.RequestNewCredential</c> method</param>
    /// <returns>The newly created user. If this value is null, an error occurred during creation</returns>
    Task<User?> CreateUser(Fido2.CredentialMakeResult credential, CredentialCreateOptions options);
}