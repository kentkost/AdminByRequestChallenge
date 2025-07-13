using System.Collections.Concurrent;

namespace AdminByRequestChallenge.API.Middlewares;

/// <summary>
/// The point of this middleware is to check if the user has exceeded their request limit
/// This middleware will also increment the number of requests made by the user.
/// As this is just for testing. It is very simple. Max requests a user will be able to make is 10 per user.
/// A better implementation would have this in a gateway where the counters are stored in a redis db and inmemory (Decorator-pattern)
/// </summary>
public class RateLimiterMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<RateLimiterMiddleware> logger;
    private int maxRequests = 10;
    private static readonly ConcurrentDictionary<string, int> counters = new();

    public RateLimiterMiddleware(RequestDelegate next, ILogger<RateLimiterMiddleware> logger)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
        this.logger = logger;
    }

    // This is a very lazy way of doing it. Window continously expands.
    public async Task Invoke(HttpContext context, IHttpContextAccessor accessor)
    {
        var username = GetClientKey(context);

        if (string.IsNullOrEmpty(username))
            goto end;

        
        var newCount = counters.AddOrUpdate(username, 1, (_, current) => current + 1);

        if (newCount > maxRequests)
        {
            logger.LogInformation("Following user has made too many request too fast: {user}", username);
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too Many Requests (limit 10). Restart API.");
            return;
        }

        end:
        await next(context);
    }

    private static string GetClientKey(HttpContext context)
    {
        const string claimUsername = "Username";
        var inviter = context.User.Claims.Where(x => x.Type == claimUsername).Select(x => x.Value).FirstOrDefault("");
        return inviter;
    }
       
}
