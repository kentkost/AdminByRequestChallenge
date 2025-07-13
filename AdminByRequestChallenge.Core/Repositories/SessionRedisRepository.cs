using AdminByRequestChallenge.Core.Interfaces;
using AdminByRequestChallenge.DataContext.Entities;
using Microsoft.Extensions.Logging;

namespace AdminByRequestChallenge.Core.Repositories;

public class SessionRedisRepository(ISessionRepository databaseRepo, ILogger<SessionRedisRepository> logger) : ISessionRepository
{
    public async Task<bool> AddSession(string username, string key, long expiration, bool isGuest = false)
    {
        logger.LogInformation("Couldn't add session redis. Because not implemented");
        return await databaseRepo.AddSession(username, key, expiration, isGuest);
    }

    public async Task<Session?> GetNonExpiredSession(string key)
    {
        logger.LogInformation("Couldn't find session in redis. Because not implemented");
        return await databaseRepo.GetNonExpiredSession(key);
    }

    public async Task InvalidateSessions(string username)
    {
        logger.LogInformation("Couldn't find invalidate sessions for user in redis. Because not implemented");
        await databaseRepo.InvalidateSessions(username);
    }

    public async Task<bool> IsSessionValid(string sessionKey)
    {
        logger.LogInformation("Couldn't get session from Redis. Because not implemented");
        return await databaseRepo.IsSessionValid(sessionKey);
    }

    public async Task<bool> MarkSessionAsInvalid(string key)
    {
        logger.LogInformation("Couldn't mark session as used in redis. Because not implemented");
        return await databaseRepo.MarkSessionAsInvalid(key);
    }

    public void SetSessionAsUsed(string key)
    {
        logger.LogInformation("Couldn't set session as used in Redis. Because not implemented");
        databaseRepo.MarkSessionAsInvalid(key);
    }
}
