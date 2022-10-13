using System.Text;
using Fido2NetLib;
using Fido2NetLib.Development;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PasskeyDemo.Interfaces;
using PasskeyDemo.Models;
using PasskeyDemo.Models.DTO;

namespace PasskeyDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IFido2 _fido2;
    private readonly IUserRepository _userRepository;

    public AuthenticationController(
        IFido2 fido2, 
        IUserRepository userRepository)
    {
        _fido2 = fido2;
        _userRepository = userRepository;
    }

    [HttpGet]
    [Route("/[controller]/UsernameAvailable")]
    public async Task<bool> IsUsernameAvailable(string username)
    {
        var doesUserExist = await _userRepository.GetUser(username);
    
        return doesUserExist is null;
    }
    
    [HttpGet]
    [Route("/[controller]/GetCredentialOptions")]
    public CredentialCreateOptions GetCredentialOptions(string username)
    {
        try
        {
            var fidoUser = new Fido2User
            {
                DisplayName = username,
                Name = username,
                Id = Encoding.UTF8.GetBytes(username)
            };
    
            var options = _fido2.RequestNewCredential(
                fidoUser,
                new List<PublicKeyCredentialDescriptor>(),
                new AuthenticatorSelection(),
                AttestationConveyancePreference.None,
                new AuthenticationExtensionsClientInputs());
            
            return options;
        }
        catch (Exception e)
        {
            return new CredentialCreateOptions { Status = "error", ErrorMessage = "Error" };
        }
    }
    
    [HttpPost]
    [Route("/[controller]/MakeCredential")]
    public async Task MakeCredential([FromBody] MakeCredentialRequestBody request)
    {
        try
        {
            var attestationObject = new AuthenticatorAttestationRawResponse()
            {
                Id = request.Id,
                RawId = request.RawId,
                Response = new()
                {
                    AttestationObject = request.AttestationObject,
                    ClientDataJson = request.ClientDataJson
                },
                Type = PublicKeyCredentialType.PublicKey
            };
    
            var credential = await _fido2.MakeNewCredentialAsync(
                attestationObject, 
                request.Options,
                (p, token) =>
                {
                    return Task.FromResult(true);
                });

            if (credential.Result is null) return;

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
                Id = request.Options.User.Id,
                Username = request.Options.User.Name,
                DisplayName = request.Options.User.DisplayName,
                Credential = storedCredential
            };
            
            await _userRepository.CreateUser(newUser);
    
            
            var ten = 10;
        }
        catch (Exception e)
        {
            var ten = 10;
        }
    }
    
    [HttpGet]
    [Route("/[controller]/GetAttestationOptions")]
    public async Task<AssertionOptions> GetAttestationOptions(string username)
    {
        var user = await _userRepository.GetUser(username);
        if (user is null) return new AssertionOptions();
        
        var allowedCredentials = new List<PublicKeyCredentialDescriptor> { new(user.Id) };
        var output = _fido2.GetAssertionOptions(allowedCredentials, UserVerificationRequirement.Preferred);
    
        return output;
    }
    
    private byte[] ConvertGarbage(string input)
    {
        var output = Base64UrlEncoder.DecodeBytes(input);
        return output;
    }
}