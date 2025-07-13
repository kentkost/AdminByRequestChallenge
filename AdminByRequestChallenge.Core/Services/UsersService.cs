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

    public async Task<string> CreateGuestUser(string inviter, List<string> claims)
    {
        var host = await repo.GetUser(inviter);
        var key = Guid.NewGuid().ToString();
        var expiration = DateTime.UtcNow;
        var tmpCode = PasswordHashProvider.GenerateSixDigitCode();
        var salt = PasswordHashProvider.GenerateSalt();
        var hashedPassword = PasswordHashProvider.HashPassword(tmpCode, salt);

        var couldCreateGuest = await repo.CreateGuestUser(host, expiration, hashedPassword, salt);

        if (!couldCreateGuest)
            throw new Exception("Couldn't create guest user");

        return tmpCode;
    }
}
