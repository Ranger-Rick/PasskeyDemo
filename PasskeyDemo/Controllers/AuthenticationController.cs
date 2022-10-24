using System.Text;
using Fido2NetLib;
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
    private readonly IWebAuthentication _authentication;
    private readonly IUserRepository _userRepository;
    private readonly IUserRegistration _userRegistration;

    public AuthenticationController(
        IUserRepository userRepository, 
        ITokenGenerator tokenGenerator, 
        IWebAuthentication authentication, 
        IUserRegistration userRegistration)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _authentication = authentication;
        _userRegistration = userRegistration;
    }

    [HttpGet]
    [Route("/[controller]/UsernameAvailable")]
    public async Task<IApiResponse<bool>> IsUsernameAvailable(string username)
    {
        if (username.Any(char.IsWhiteSpace)) return new GenericApiResponse<bool>(false, "Username cannot contain spaces", false);
        
        var user = await _userRepository.GetUser(username);

        var successful = user is null;
        var message = successful
            ? "There are no users with the username you provided. You can register with this username!"
            : $"The username {username} is already in use, please select a different username";

        var output = new GenericApiResponse<bool>(successful, message, successful);
    
        return output;
    }
    
    [HttpGet]
    [Route("/[controller]/GetCredentialOptions")]
    public async Task<IApiResponse<CredentialCreateOptions>> GetCredentialOptions(string username)
    {
        try
        {
            var fidoUser = new Fido2User
            {
                DisplayName = username,
                Name = username,
                Id = Encoding.UTF8.GetBytes(username)
            };

            var options = await _authentication.GetCredentialOptions(fidoUser);

            var output = new GenericApiResponse<CredentialCreateOptions>(options);
            
            return output;
        }
        catch (Exception e)
        {
            return new GenericApiResponse<CredentialCreateOptions>(null, "An error occurred", false);
        }
    }
    
    [HttpPost]
    [Route("/[controller]/MakeCredential")]
    public async Task<IApiResponse<LoginResponseDto>> MakeCredential([FromBody] MakeCredentialDto request)
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

        var credential = await _authentication.MakeCredential(attestationObject, request.Options);

        if (credential.Result is null) return new GenericApiResponse<LoginResponseDto>(null, "An Error Occurred when making the credential", false);

        var newUser = await _userRegistration.CreateUser(credential, request.Options);

        if (newUser is null) return new GenericApiResponse<LoginResponseDto>(null, "An error occurred when creating the user", false);

        var token = _tokenGenerator.GenerateToken(newUser);
        
        var output = new LoginResponseDto
        {
            UserId = Encoding.ASCII.GetString(newUser.Id),
            Username = newUser.Username,
            DisplayName = newUser.DisplayName,
            Color = newUser.Color,
            Token = token
        };

        return new GenericApiResponse<LoginResponseDto>(output);

    }
    
    [HttpGet]
    [Route("/[controller]/GetAssertionOptions")]
    public async Task<IApiResponse<AssertionOptions>> GetAssertionOptions(string username)
    {
        var user = await _userRepository.GetUser(username);
        if (user is null)
        {
            var message = "Unable to find an account with the username provided. Do you need to register first?";
            return new GenericApiResponse<AssertionOptions>(null, message, false);
        }
        var output = await _authentication.GetAssertionOptions(user);
        return new GenericApiResponse<AssertionOptions>(output);
    }

    [HttpPost]
    [Route("/[controller]/MakeAssertion")]
    public async Task<IApiResponse<LoginResponseDto>> MakeAssertion([FromBody] MakeAssertionDto assertionDto)
    {
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

        var response = await _authentication.MakeAssertion(assertionDto.Id, assertionRawResponse, assertionDto.AssertionOptions);

        if (!response.CredentialId.AsSpan().SequenceEqual(assertionDto.Id))
        {
            return new GenericApiResponse<LoginResponseDto>(null, "An error occurred when making the assertion", false);
        }
        
        var user = await _userRepository.GetUser(assertionDto.UserHandle);
        if (user is null) return new GenericApiResponse<LoginResponseDto>(null, "Unable to get the user to generate a token", false);
        var token = _tokenGenerator.GenerateToken(user);

        var output = new LoginResponseDto
        {
            UserId = Encoding.ASCII.GetString(user.Id),
            Username = user.Username,
            DisplayName = user.DisplayName,
            Color = user.Color,
            Token = token
        };

        return new GenericApiResponse<LoginResponseDto>(output);
    }
}