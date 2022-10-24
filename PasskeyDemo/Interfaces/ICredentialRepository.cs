using Fido2NetLib.Development;

namespace PasskeyDemo.Interfaces;

public interface ICredentialRepository
{
    Task<StoredCredential> GetCredentialById(byte[] id);
    Task UpdateSignatureCount(byte[] credentialId);
}