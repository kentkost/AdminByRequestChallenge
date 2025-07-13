using AdminByRequestChallange.Contracts;
using AdminByRequestChallenge.Core.Interfaces;
using AdminByRequestChallenge.Core.Providers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace AdminByRequestChallenge.Core.Services;

public class AuthService(IUserRepository userRepo, IJwtProvider jwtProvider, ISessionRepository sessionStore) : IAuthService
{
    const int sessionLifetime = 60;
    const int guestSessionLifetime = 10;
    public async Task<SessionResponse> CreateSession(string username, string password)
    {
        bool passwordIsCorrect = await AuthenticateUser(username, password);
        if(!passwordIsCorrect)
            throw new Exception("Incorrect credentials");

        // Generate SessionID and save it to repository.
        var key = Guid.NewGuid().ToString();
        var expiration = DateTimeOffset.UtcNow.AddMinutes(sessionLifetime).ToUnixTimeSeconds();

        var resp = new SessionResponse() { SessionKey = key, Expiration = expiration};
        var couldCreateSession = await sessionStore.AddSession(username, key, expiration);

        if(!couldCreateSession)
            throw new Exception("Session Creation failed");

        return resp;
    }

    public async Task<SessionResponse> CreateGuestSession(string inviter, List<string> claims)
    {
        var key = Guid.NewGuid().ToString();
        var expiration = DateTimeOffset.UtcNow.AddMinutes(guestSessionLifetime).ToUnixTimeSeconds();
        var sess = new SessionResponse() { SessionKey = key, Expiration = expiration };

        var couldCreateSession = await sessionStore.AddSession(inviter, key, expiration, true);

        if (!couldCreateSession)
            throw new Exception("Session Creation failed");

        return sess;
    }

    public async Task<string> CreateJwt(string username, string password)
    {
        bool passwordIsCorrect = await this.AuthenticateUser(username, password);
        
        if (!passwordIsCorrect)
            throw new UnauthorizedAccessException("Incorrect credentials");

        var user = await userRepo.GetUser(username);
        var sessionKey = Guid.NewGuid().ToString();
        var expiration = DateTime.UtcNow.AddMinutes(sessionLifetime).Ticks;

        var jwt = jwtProvider.CreateToken(sessionKey, expiration, user);
        
        var couldCreateSession = await sessionStore.AddSession(username, sessionKey, expiration);
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
}


