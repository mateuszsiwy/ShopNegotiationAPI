
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ShopNegotiationAPI.Application.Interfaces.Services;
using ShopNegotiationAPI.Domain.Models;
using ShopNegotiationAPI.DTOs;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] ShopNegotiationAPI.DTOs.LoginRequest request)
    {
        var user = await _authService.ValidateUser(request.Username, request.Password);
        if (user == null)
            return Unauthorized();

        var token = _authService.GenerateJwtToken(user);
        return Ok(new { Token = token });
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] ShopNegotiationAPI.DTOs.RegisterRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "User" 
        };

        await _authService.RegisterUser(user);
        return Ok();
    }
}