using AdminByRequestChallenge.DataContext.Entities;

namespace AdminByRequestChallenge.Core.Interfaces;

public interface IUserRepository
{
    Task<bool> CreateGuestUser(User host, DateTime expiration, byte[] hashedPassword, byte[] salt);
    Task<bool> CreateUser(User entUser);
    Task<List<GuestUser>> GetGuestUsers(string username);
    Task<User> GetUser(string username);
    void MarkGuestUserPasswordAsUsed(int id);
}