using System.Text.Json.Serialization; //This is important
using Fido2NetLib;

namespace PasskeyDemo.Models.DTO;

public class PublicKeyCredentialDto
{
    [JsonConverter(typeof(Base64UrlConverter))]
    public byte[] Id { get; init; }
    
    [JsonConverter(typeof(Base64UrlConverter))]
    public byte[] RawId { get; init; }
    
    [JsonConverter(typeof(Base64UrlConverter))]
    public byte[] ClientDataJson { get; init; }
    
    
    
    
}