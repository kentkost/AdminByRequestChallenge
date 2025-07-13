using AdminByRequestChallange.Contracts;
using AdminByRequestChallenge.Core.Interfaces;
using AdminByRequestChallenge.DataContext;
using AdminByRequestChallenge.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminByRequestChallenge.Core.Repositories;

public class SessionDatabaseRepository(AuthContext context) : ISessionRepository
{
    public async Task<bool> AddSession(string username, string key, long expiration, bool isGuest = false)
    {
        var session = new Session() { Username = username, SessionKey = key, Expiration = expiration, HasBeenUsed = false, IsGuest = isGuest };
        await context.Sessions.AddAsync(session);
        return await context.SaveChangesAsync() == 1;
    }

    public async Task<Session?> GetNonExpiredSession(string key)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var ses = await context.Sessions.Where(x => x.SessionKey == key &&  x.Expiration >= now).FirstOrDefaultAsync();
        return ses; 
    }
    

    public async Task<bool> MarkSessionAsUsed(string key)
        => await context.Sessions.Where(x => x.SessionKey == key).ExecuteUpdateAsync(x=> x.SetProperty(x=> x.HasBeenUsed, true)) == 1;

    public async Task InvalidateSessions(string username)
        => await context.Sessions.Where(x => x.Username == username).ExecuteUpdateAsync(x => x.SetProperty(x => x.Expiration, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));

    public void SetSessionAsUsed(string key)
        => context.Sessions.Where(x => x.SessionKey == key).ExecuteUpdate(x => x.SetProperty(x => x.HasBeenUsed, true));
}
