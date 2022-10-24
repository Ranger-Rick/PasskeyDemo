using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasskeyDemo.Interfaces;
using PasskeyDemo.Models;

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
    public async Task<IApiResponse<string>> GetColor(string userId)
    {
        var id = Encoding.ASCII.GetBytes(userId);
        
        var user = await _userRepository.GetUser(id);

        var output = user is null ? "" : user.Color;

        return new GenericApiResponse<string>(output);
    }

    [HttpPost]
    [Route("/[controller]/UpdateColor")]
    public async Task<IApiResponse> UpdateColor(string userId, string color)
    {
        var id = Encoding.ASCII.GetBytes(userId);
        var user = await _userRepository.GetUser(id);
        if (user is null)
        {
            return new GenericApiResponse(false, "Could not find a user to update");
        }

        user.Color = $"#{color}";

        await _userRepository.UpdateUser(user);

        return new GenericApiResponse();
    }
}