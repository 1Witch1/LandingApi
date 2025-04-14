using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using LandingApi.Models;
using LandingApi.DTOs;
using Microsoft.EntityFrameworkCore;
using LandingApi;
using LandingApi.Services;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var token = await _authService.Authenticate(model.Name, model.Password);
        if (token == null)
            return Unauthorized();

        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new User
        {
            Name = model.Name,
            RoleId = model.RoleId
        };

        var result = await _authService.Register(user, model.Password);
        if (!result)
            return BadRequest("User already exists");

        return Ok();
    }
}

public class LoginModel
{
    public string Name { get; set; } = string.Empty; // Изменено с Username на Name
    public string Password { get; set; } = string.Empty;
}