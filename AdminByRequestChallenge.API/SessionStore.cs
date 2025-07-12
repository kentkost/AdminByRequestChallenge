using System.Collections.Concurrent;

namespace AdminByRequestChallenge.API;

public interface ISessionStore
{
    /// returns null if the key is invalid / expired
    /// TODO: Fix this. USername and sessionkey should be required.
    UserInfo? GetUser(string sessionKey);

    /// creates & persists a brand-new key
    string CreateSession(string username, string password);
}

public sealed class InMemorySessionStore : ISessionStore
{
    private readonly ConcurrentDictionary<string, (UserInfo user, DateTime expires)> _sessions = new();

    public string CreateSession(string username, string password)
    {
        // TODO replace with your real user-lookup
        if (username != "demo" || password != "demo") throw new UnauthorizedAccessException();

        var key = Guid.NewGuid().ToString();
        _sessions[key] = (new UserInfo(Guid.NewGuid(), username), DateTime.UtcNow.AddHours(1));
        return key;
    }

    public UserInfo? GetUser(string sessionKey)
    {
        if (_sessions.TryGetValue(sessionKey, out var tuple) && tuple.expires > DateTime.UtcNow)
            return tuple.user;

        return null;
    }
}

public record UserInfo(Guid Id, string Username);
