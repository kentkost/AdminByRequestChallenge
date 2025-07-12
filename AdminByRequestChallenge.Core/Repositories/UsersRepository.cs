using AdminByRequestChallenge.Core.Interfaces;
using AdminByRequestChallenge.DataContext;
using AdminByRequestChallenge.DataContext.Entities;

namespace AdminByRequestChallenge.Core.Repositories;

public class UsersRepository(AuthContext context) : IUsersRepository
{
    public async Task<bool> CreateUser(User entUser)
    {
        await context.Users.AddAsync(entUser);
        return await context.SaveChangesAsync() == 1;
    }
}