using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasskeyDemo.Interfaces;

namespace PasskeyDemo.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ColorController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public ColorController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    [Route("/[controller]/GetColor")]
    public async Task<string> GetColor(string userId)
    {
        var id = Encoding.ASCII.GetBytes(userId);
        
        var user = await _userRepository.GetUser(id);

        return user is null ? "" : user.Color;
    }

    [HttpPost]
    [Route("/[controller]/UpdateColor")]
    public async Task<IActionResult> UpdateColor(string userId, string color)
    {
        var id = Encoding.ASCII.GetBytes(userId);
        var user = await _userRepository.GetUser(id);
        if (user is null)
        {
            return NotFound("Could not find a user to update");
        }

        user.Color = $"#{color}";

        await _userRepository.UpdateUser(user);
        
        return Ok();
    }
}