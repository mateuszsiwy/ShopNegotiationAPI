using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Application.Interfaces.Services;

public interface IAuthService
{
    Task<User?> ValidateUser(string username, string password);
    string GenerateJwtToken(User user);
}