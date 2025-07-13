using AdminByRequestChallenge.Contracts;
using AdminByRequestChallenge.Core.Interfaces;
using AdminByRequestChallenge.Core.Providers;
using AdminByRequestChallenge.DataContext.Entities;

namespace AdminByRequestChallenge.Core.Services;

public class UsersService(IUserRepository repo) : IUsersService
{
    public async Task<bool> CreateUser(UserCreationDTO user)
    {
        var salt = PasswordHashProvider.GenerateSalt();
        var entUser = new User()
        {
            Username = user.Username,
            PasswordHash = PasswordHashProvider.HashPassword(user.Password, salt),
            Salt = salt
        };

        return await repo.CreateUser(entUser);
    }
}
