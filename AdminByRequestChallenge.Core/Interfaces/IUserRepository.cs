using AdminByRequestChallenge.DataContext.Entities;

namespace AdminByRequestChallenge.Core.Interfaces;

public interface IUserRepository
{
    Task<bool> CreateUser(User entUser);
    Task<User> GetUser(string username);
}