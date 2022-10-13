using System.Text.Json.Serialization;
using Fido2NetLib;

namespace PasskeyDemo.Models.DTO;

public class MakeCredentialRequestBody
{
    // public AuthenticatorAttestationRawResponse AttestationRawResponse { get; init; }
    // public CredentialCreateOptions Options { get; init; }
    
    [JsonConverter(typeof(Base64UrlConverter))]
    public byte[] Id { get; init; }
    
    [JsonConverter(typeof(Base64UrlConverter))]
    public byte[] RawId { get; init; }
    
    [JsonConverter(typeof(Base64UrlConverter))]
    public byte[] AttestationObject { get; init; }
    
    [JsonConverter(typeof(Base64UrlConverter))]
    public byte[] ClientDataJson { get; init; }
    public CredentialCreateOptions Options { get; init; }
}