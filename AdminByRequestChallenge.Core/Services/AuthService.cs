using AdminByRequestChallenge.Core.Interfaces;
using AdminByRequestChallenge.Core.Providers;

namespace AdminByRequestChallenge.Core.Services;

public class AuthService(IUserRepository userRepo, IJwtProvider jwtProvider, ISessionRepository sessionStore) : IAuthService
{
    const int sessionLifetime = 60;
    const int guestSessionLifetime = 10;

    public async Task<string> CreateJwt(string username, string password)
    {
        bool passwordIsCorrect = await this.AuthenticateUser(username, password);
        
        if (!passwordIsCorrect)
            throw new UnauthorizedAccessException("Incorrect credentials");

        return await CreateSessionAndJwt(username, sessionLifetime);
    }

    public async Task<string> CreateGuestJwt(string username, string password)
    {
        bool passwordIsCorrect = await this.AuthenticateGuestUser(username, password);

        if (!passwordIsCorrect)
            throw new UnauthorizedAccessException("Incorrect credentials");
        
        return await CreateSessionAndJwt(username, guestSessionLifetime);
    }

    private async Task<string> CreateSessionAndJwt(string username, int expirationTime)
    {
        var user = await userRepo.GetUser(username);
        var sessionKey = Guid.NewGuid().ToString();
        var expiration = DateTime.UtcNow.AddMinutes(expirationTime).Ticks;

        var jwt = jwtProvider.CreateToken(sessionKey, expiration, user);

        var couldCreateSession = await sessionStore.AddSession(username, sessionKey, expiration, true);
        if (!couldCreateSession)
            throw new UnauthorizedAccessException("Session Creation failed");

        return jwt;
    }

    public async Task<bool> AuthenticateUser(string username, string password)
    {
        var user = await userRepo.GetUser(username);
        var hashToMatch = PasswordHashProvider.HashPassword(password, user.Salt);
        return hashToMatch.SequenceEqual(user.PasswordHash);
    }

    public async Task<bool> AuthenticateGuestUser(string username, string password)
    {
        var users = await userRepo.GetGuestUsers(username);
        foreach(var user in users)
        {
            var hashToMatch = PasswordHashProvider.HashPassword(password, user.Salt);
            var matches = hashToMatch.SequenceEqual(user.PasswordHash);
            if (matches)
            {
                userRepo.MarkGuestUserPasswordAsUsed(user.Id);
                return true;
            }
        }

        return false;
    }
}


