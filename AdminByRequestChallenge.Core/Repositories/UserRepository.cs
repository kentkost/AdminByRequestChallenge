using AdminByRequestChallenge.Core.Interfaces;
using AdminByRequestChallenge.DataContext;
using AdminByRequestChallenge.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminByRequestChallenge.Core.Repositories;

public class UserRepository(AuthContext context) : IUserRepository
{
    public async Task<bool> CreateUser(User entUser)
    {
        entUser.Username = entUser.Username.ToLower().Trim();
        await context.Users.AddAsync(entUser);
        return await context.SaveChangesAsync() == 1;
    }

    public async Task<User> GetUser(string username)
        => await context.Users.Where(x => x.Username == username.ToLower().Trim()).FirstAsync();
}