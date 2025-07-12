using System.Collections.Concurrent;

namespace AdminByRequestChallenge.API;

public interface ISessionStore
{
    /// returns null if the key is invalid / expired
    /// TODO: Fix this. USername and sessionkey should be required.
    UserInfo? GetUser(string sessionKey);

    /// creates & persists a brand-new key
    SessionResponse CreateSession(string username, string password);
}

public sealed class InMemorySessionStore : ISessionStore
{
    private readonly ConcurrentDictionary<string, (UserInfo user, DateTimeOffset expires)> _sessions = new();

    public SessionResponse CreateSession(string username, string password)
    {
        // TODO replace with your real user-lookup
        if (username != "demo" || password != "demo") throw new UnauthorizedAccessException();

        var key = Guid.NewGuid().ToString();
        DateTimeOffset expiration = DateTime.UtcNow.AddHours(1);
        _sessions[key] = (new UserInfo(Guid.NewGuid(), username), expiration);
        var resp = new SessionResponse(key, expiration.ToUnixTimeSeconds());
        return resp;
    }

    public UserInfo? GetUser(string sessionKey)
    {
        if (_sessions.TryGetValue(sessionKey, out var tuple) && tuple.expires > DateTime.UtcNow)
            return tuple.user;

        return null;
    }
}

public record UserInfo(Guid Id, string Username);

public record SessionResponse(string session, long expiration);