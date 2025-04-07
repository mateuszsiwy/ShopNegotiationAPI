using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopNegotiationAPI.Application.Interfaces.Repositories;
using ShopNegotiationAPI.Application.Interfaces.Services;
using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<User?> ValidateUser(string username, string password)
    {
        var user = await _userRepository.GetByUsername(username);
        if (user == null)
            return null;

        // Verify password hash
        if (!VerifyPasswordHash(password, user.PasswordHash))
            return null;

        return user;
    }

    public string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool VerifyPasswordHash(string password, string storedHash)
    {
        // Implement password verification
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
    
    public async Task RegisterUser(User user)
    {
        await _userRepository.AddUser(user);
    }
}