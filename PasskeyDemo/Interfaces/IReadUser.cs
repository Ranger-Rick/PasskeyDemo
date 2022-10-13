using PasskeyDemo.Models;

namespace PasskeyDemo.Interfaces;

public interface IReadUser
{
    Task<User?> GetUser(string username);
}