using Fido2NetLib;
using Fido2NetLib.Objects;
using PasskeyDemo.Interfaces;
using PasskeyDemo.Models;

namespace PasskeyDemo.Services;

public class WebAuthentication : IWebAuthentication
{
    private readonly IFido2 _fido2;
    private readonly ICredentialRepository _credentialRepository;

    public WebAuthentication(
        IFido2 fido2, 
        ICredentialRepository credentialRepository)
    {
        _fido2 = fido2;
        _credentialRepository = credentialRepository;
    }

    public Task<CredentialCreateOptions> GetCredentialOptions(Fido2User user)
    {
        var options = _fido2.RequestNewCredential(
            user,
            new List<PublicKeyCredentialDescriptor>(),
            new AuthenticatorSelection(),
            AttestationConveyancePreference.None,
            new AuthenticationExtensionsClientInputs());
            
        //Leaving this method's return type as a Task<> because there may be different, or asynchronous ways of obtaining options
        return Task.FromResult(options);
    }

    public async Task<Fido2.CredentialMakeResult> MakeCredential(AuthenticatorAttestationRawResponse attestationResponse, CredentialCreateOptions options)
    {
        var credential = await _fido2.MakeNewCredentialAsync(
            attestationResponse, 
            options,
            (p, token) =>
            {
                //TODO: Figure out what to really do here
                return Task.FromResult(true);
            });

        return credential;
    }

    public Task<AssertionOptions> GetAssertionOptions(User user)
    {
        var allowedCredentials = new List<PublicKeyCredentialDescriptor> { user.Credential.Descriptor };
        var output = _fido2.GetAssertionOptions(allowedCredentials, UserVerificationRequirement.Preferred);
    
        return Task.FromResult(output);
    }

    public async Task<AssertionVerificationResult> MakeAssertion(byte[] credentialId, AuthenticatorAssertionRawResponse assertionResponse, AssertionOptions assertionOptions)
    {
        var credential = await _credentialRepository.GetCredentialById(credentialId);

        var storedCounter = credential.SignatureCounter;
        
        // 4. Create callback to check if userhandle owns the credentialId
        IsUserHandleOwnerOfCredentialIdAsync callback = static async (args, cancellationToken) =>
        {
            // var storedCreds = await DemoStorage.GetCredentialsByUserHandleAsync(args.UserHandle, cancellationToken);
            // return storedCreds.Exists(c => c.Descriptor.Id.SequenceEqual(args.CredentialId));
            return true;
        };

        //I'm not sure what happens if the Assertion fails here...
        var response = await _fido2.MakeAssertionAsync(assertionResponse, assertionOptions, credential.PublicKey, storedCounter, callback);

        //Documentation says we need to update the Signature Counter here. I'm not sure why though
        //TODO: Update the SignatureCounter
        
        return response;
    }
}