using AdminByRequestChallenge.Core.Interfaces;

namespace AdminByRequestChallenge.API.Middlewares;

/// <summary>
/// The point of this middleware is to check if the session on the JWT has since been invalidated.
/// In which case the user will not be able to connect.
/// </summary>
public class SessionValidatorMiddleware : IMiddleware
{
    private readonly ISessionRepository sessionRepo;

    public SessionValidatorMiddleware(ISessionRepository sessionRepo)
    {
        this.sessionRepo = sessionRepo;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
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
