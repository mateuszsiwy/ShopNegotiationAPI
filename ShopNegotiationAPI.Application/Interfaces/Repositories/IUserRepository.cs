using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> GetByUsername(string username);
    Task AddUser(User user);
}