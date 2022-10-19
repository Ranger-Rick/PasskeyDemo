using System.Text.Json;
using Fido2NetLib.Development;
using PasskeyDemo.Interfaces;
using PasskeyDemo.Models;

namespace PasskeyDemo.Services;

public class DemoUserRepository : IUserRepository, ICredentialRepository
{
    private const string FileName = "TempDatabase.json";

    public async Task CreateUser(User user)
    {
        var allUsers = await GetAllUsers();

        var doesUserExist = UserExists(allUsers, user);

        if (doesUserExist) return;
        
        allUsers.Add(user);

        var writeString = JsonSerializer.Serialize(allUsers);
        
        await File.WriteAllTextAsync(FileName, writeString);
    }

    public async Task UpdateUser(User user)
    {
        var allUsers = await GetAllUsers();
        allUsers.RemoveAll(u => u.Id.AsSpan().SequenceEqual(user.Id));
        
        allUsers.Add(user);

        var writeString = JsonSerializer.Serialize(allUsers);
        await File.WriteAllTextAsync(FileName, writeString);
    }

    public async Task<User?> GetUser(string username)
    {
        var allUsers = await GetAllUsers();
        var matchingUser = allUsers.FirstOrDefault(u => u.Username == username);

        return matchingUser;
    }

    public async Task<User?> GetUser(byte[] userId)
    {
        var allUsers = await GetAllUsers();
        var matchingUser = allUsers.FirstOrDefault(u => u.Id.AsSpan().SequenceEqual(userId));

        return matchingUser;
    }

    public async Task<bool> IsUsernameAvailable(string username)
    {
        var allUsers = await GetAllUsers();
        var matchingUser = allUsers.FirstOrDefault(u => u.Username == username);

        return matchingUser is not null;
    }

    public async Task<StoredCredential> GetCredentialById(byte[] id)
    {
        var users = await GetAllUsers();
        var credential = users.FirstOrDefault(c => c.Credential.Descriptor.Id.AsSpan().SequenceEqual(id));

        if (credential is null) return new StoredCredential();

        return credential.Credential;
    }

    private bool UserExists(List<User> existingUsers, User user)
    {
        return existingUsers.Any(u => u.Username == user.Username);
    }
    
    private async Task<List<User>> GetAllUsers()
    {
        try
        {
            var allText = await File.ReadAllTextAsync(FileName);
            var users = JsonSerializer.Deserialize<List<User>>(allText);
            return users ?? new List<User>();
        }
        catch (Exception e)
        {
            return new List<User>();
        }
    }

    
}