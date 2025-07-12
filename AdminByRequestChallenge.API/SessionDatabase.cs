namespace AdminByRequestChallenge.API;

public static class SessionDatabase
{
    public static List<LoginSession> Sessions = new List<LoginSession>();
    private static TimeSpan sessionLifeTime = new TimeSpan(0, 45, 0);

    public static LoginSession CreateSession(string username, string password)
    {
        var res = UserDatabase.VerifyPassword(username, password);
        if (!res)
            return null;

        var session = new LoginSession()
        {
            UserName = username,
            SessionID = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
            DeviceInfo = "Firefox, OS, Version, IP, User Agent, Screen resolution, Timezone",
            Created = DateTime.UtcNow,
            Expiration = DateTime.UtcNow.Add(sessionLifeTime),
        };
        Sessions.Add(session);

        return session;
    }

    public static LoginSession? ValidateSession(string username, string sessionID)
    {
        var now = DateTime.UtcNow;
        var session = Sessions.Where(x => x.UserName == username)
                              .Where(x => x.SessionID == sessionID)
                              .Where(x => now <= x.Expiration)
                              .FirstOrDefault();

        return session;
    }

    public static LoginSession RefreshExpiredSession(string username, string sessionID)
    {
        // Delete old session
        // Create new session without the need for password
        // Return new session
        return null;
    }

    public static bool DeleteSession(string? sessionID)
    {
        var ses = Sessions.Where(x => x.SessionID == sessionID).FirstOrDefault();
        if (ses != null)
            return Sessions.Remove(ses);
        return false;
    }
}

public class LoginSession
{
    public string UserName { get; set; } = string.Empty;
    public string SessionID { get; set; } = string.Empty;
    public string DeviceInfo { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime Expiration { get; set; }
}
