using AdminByRequestChallenge.Core.Interfaces;
using AdminByRequestChallenge.DataContext;
using AdminByRequestChallenge.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminByRequestChallenge.Core.Repositories;

public class SessionDatabaseRepository(AuthContext context) : ISessionRepository
{
    public async Task<bool> AddSession(string username, string key, long expiration, bool isGuest = false)
    {
        var session = new Session() { Username = username, SessionKey = key, Expiration = expiration, Valid = true, IsGuest = isGuest };
        await context.Sessions.AddAsync(session);
        return await context.SaveChangesAsync() == 1;
    }

    public async Task<Session?> GetNonExpiredSession(string key)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var ses = await context.Sessions.Where(x => x.SessionKey == key &&  x.Expiration >= now).FirstOrDefaultAsync();
        return ses; 
    }

    public async Task<bool> MarkSessionAsInvalid(string key)
        => await context.Sessions.Where(x => x.SessionKey == key).ExecuteUpdateAsync(x=> x.SetProperty(x=> x.Valid, false)) == 1;

    public async Task InvalidateSessions(string username)
        => await context.Sessions.Where(x => x.Username == username).ExecuteUpdateAsync(x => x.SetProperty(x => x.Expiration, DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                                                                                              .SetProperty(x=> x.Valid, false));
}
