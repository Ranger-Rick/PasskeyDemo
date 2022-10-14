using System.Text.Json.Serialization;
using Fido2NetLib;

namespace PasskeyDemo.Models.DTO;

public class MakeCredentialDto : PublicKeyCredentialDto
{
    [JsonConverter(typeof(Base64UrlConverter))]
    public byte[] AttestationObject { get; set; }
    
    public CredentialCreateOptions Options { get; init; }
}