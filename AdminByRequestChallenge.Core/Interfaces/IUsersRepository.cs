using AdminByRequestChallenge.DataContext.Entities;

namespace AdminByRequestChallenge.Core.Interfaces;

public interface IUsersRepository
{
    Task<bool> CreateUser(User entUser);
}