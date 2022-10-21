using System.Text;
using Fido2NetLib;
using Fido2NetLib.Development;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Mvc;
using PasskeyDemo.Interfaces;
using PasskeyDemo.Models;
using PasskeyDemo.Models.DTO;

namespace PasskeyDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IFido2 _fido2;
    private readonly IUserRepository _userRepository;
    private readonly ICredentialRepository _credentialRepository;

    public AuthenticationController(
        IFido2 fido2, 
        IUserRepository userRepository, 
        ICredentialRepository credentialRepository, 
        ITokenGenerator tokenGenerator)
    {
        _fido2 = fido2;
        _userRepository = userRepository;
        _credentialRepository = credentialRepository;
        _tokenGenerator = tokenGenerator;
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
    public async Task<LoginResponseDto> MakeCredential([FromBody] MakeCredentialDto request)
    {
        var attestationObject = new AuthenticatorAttestationRawResponse
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

        if (credential.Result is null) return new LoginResponseDto(false);

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

        var token = _tokenGenerator.GenerateToken(newUser);
        
        var output = new LoginResponseDto
        {
            UserId = Encoding.ASCII.GetString(newUser.Id),
            Username = newUser.Username,
            DisplayName = newUser.DisplayName,
            Color = newUser.Color,
            Token = token
        };

        return output;

    }
    
    [HttpGet]
    [Route("/[controller]/GetAssertionOptions")]
    public async Task<AssertionOptions> GetAttestationOptions(string username)
    {
        var user = await _userRepository.GetUser(username);
        if (user is null) return new AssertionOptions();
        
        var allowedCredentials = new List<PublicKeyCredentialDescriptor> { user.Credential.Descriptor };
        var output = _fido2.GetAssertionOptions(allowedCredentials, UserVerificationRequirement.Preferred);
    
        return output;
    }

    [HttpPost]
    [Route("/[controller]/MakeAssertion")]
    public async Task<LoginResponseDto> MakeAssertion([FromBody] MakeAssertionDto assertionDto)
    {
        var credential = await _credentialRepository.GetCredentialById(assertionDto.Id);

        var storedCounter = credential.SignatureCounter;
        
        // 4. Create callback to check if userhandle owns the credentialId
        IsUserHandleOwnerOfCredentialIdAsync callback = static async (args, cancellationToken) =>
        {
            // var storedCreds = await DemoStorage.GetCredentialsByUserHandleAsync(args.UserHandle, cancellationToken);
            // return storedCreds.Exists(c => c.Descriptor.Id.SequenceEqual(args.CredentialId));
            return true;
        };

        var assertionRawResponse = new AuthenticatorAssertionRawResponse
        {
            Id = assertionDto.Id,
            RawId = assertionDto.RawId,
            Type = PublicKeyCredentialType.PublicKey,
            Response = new AuthenticatorAssertionRawResponse.AssertionResponse
            {
                Signature = assertionDto.Signature,
                AuthenticatorData = assertionDto.AuthenticatorData,
                ClientDataJson = assertionDto.ClientDataJson,
                UserHandle = assertionDto.UserHandle
            }
        };

        var response = await _fido2.MakeAssertionAsync(assertionRawResponse, assertionDto.AssertionOptions, credential.PublicKey, storedCounter, callback);
        
        //Documentation says we need to update the Signature Counter here. I'm not sure why though
        //TODO: Update the SignatureCounter

        var user = await _userRepository.GetUser(assertionDto.UserHandle);
        if (user is null) return new LoginResponseDto(false);
        var token = _tokenGenerator.GenerateToken(user);

        var output = new LoginResponseDto
        {
            UserId = Encoding.ASCII.GetString(user.Id),
            Username = user.Username,
            DisplayName = user.DisplayName,
            Color = user.Color,
            Token = token
        };

        return output;
    }
}