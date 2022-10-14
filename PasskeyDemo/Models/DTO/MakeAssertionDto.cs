using System.Text.Json.Serialization;
using Fido2NetLib;

namespace PasskeyDemo.Models.DTO;

public class MakeAssertionDto : PublicKeyCredentialDto
{
    [JsonConverter(typeof(Base64UrlConverter))]
    public byte[] AuthenticatorData { get; init; }  
    
    [JsonConverter(typeof(Base64UrlConverter))]
    public byte[] Signature { get; init; }
    
    [JsonConverter(typeof(Base64UrlConverter))]
    public byte[] UserHandle { get; init; }
    
    public AssertionOptions AssertionOptions { get; init; }
}