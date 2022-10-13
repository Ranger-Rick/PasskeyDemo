using System.Text.Json;
using PasskeyDemo.Interfaces;
using PasskeyDemo.Models;

namespace PasskeyDemo.Services;

public class DemoUserRepository : IUserRepository
{
    private const string FileName = "TempDatabase.txt";

    public async Task CreateUser(User user)
    {
        var allUsers = await GetAllUsers();

        var doesUserExist = UserExists(allUsers, user);

        if (doesUserExist) return;
        
        allUsers.Add(user);

        var writeSting = JsonSerializer.Serialize(allUsers);
        
        await File.WriteAllTextAsync(FileName, writeSting);
    }

    public Task UpdateUser(User user)
    {
        var userString = JsonSerializer.Serialize(user);
        return Task.CompletedTask;
    }

    public async Task<User?> GetUser(string username)
    {
        var allUsers = await GetAllUsers();
        var matchingUser = allUsers.FirstOrDefault(u => u.Username == username);

        return matchingUser;
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