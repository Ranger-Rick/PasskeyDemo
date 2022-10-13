using PasskeyDemo.Models;

namespace PasskeyDemo.Interfaces;

public interface IWriteUser
{
    Task CreateUser(User user);
    Task UpdateUser(User user);
}