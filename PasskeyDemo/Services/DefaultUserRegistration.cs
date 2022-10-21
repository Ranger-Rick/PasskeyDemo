using Fido2NetLib;
using Fido2NetLib.Development;
using Fido2NetLib.Objects;
using PasskeyDemo.Interfaces;
using PasskeyDemo.Models;

namespace PasskeyDemo.Services;

public class DefaultUserRegistration : IUserRegistration
{
    private readonly IUserRepository _userRepository;

    public DefaultUserRegistration(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> CreateUser(Fido2.CredentialMakeResult credential, CredentialCreateOptions options)
    {
        if (credential.Result == null) return null;
        
        var storedCredential = new StoredCredential
        {
            Descriptor = new PublicKeyCredentialDescriptor(credential.Result.CredentialId),
            PublicKey = credential.Result.PublicKey,
            UserHandle = credential.Result.User.Id,
            SignatureCounter = credential.Result.Counter,
            CredType = credential.Result.CredType,
            RegDate = DateTime.Now,
            AaGuid = credential.Result.Aaguid
        };
        
        var newUser = new User
        {
            Id = options.User.Id,
            Username = options.User.Name,
            DisplayName = options.User.DisplayName,
            Credential = storedCredential
        };
        
        await _userRepository.CreateUser(newUser);

        return newUser;
    }
}