using AdminByRequestChallenge.Core.Interfaces;

namespace AdminByRequestChallenge.API.Middlewares;

/// <summary>
/// The point of this middleware is to check if the session on the JWT has since been invalidated.
/// In which case the user will not be able to connect.
/// </summary>
public class SessionValidatorMiddleware
{
    private readonly RequestDelegate next;
    private readonly ISessionRepository sessionRepo;

    public SessionValidatorMiddleware(RequestDelegate next, ISessionRepository sessionRepo)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
        this.sessionRepo = sessionRepo;
    }

    public async Task Invoke(HttpContext context, IHttpContextAccessor accessor)
    {
        if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
        {
            var sessionKey = context.User.Claims.Where(x => x.Type == "SessionKey").Select(x => x.Value).First();
            bool sessionsIsValid = await sessionRepo.IsSessionValid(sessionKey);

            if (sessionsIsValid)
                goto end;

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Session is no longer valid.");
            return;
        }

        end:
        await next(context);
    }
}
