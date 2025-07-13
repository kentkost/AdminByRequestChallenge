namespace AdminByRequestChallenge.API.Middlewares;

/// <summary>
/// The point of this middleware is to check if the session on the JWT has since been invalidated.
/// In which case 
/// </summary>
public class SessionValidatorMiddleware
{
    private readonly RequestDelegate next;

    public SessionValidatorMiddleware(RequestDelegate next)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext context, IHttpContextAccessor accessor)
    {
        bool sessionsIsValid = true;

        if (sessionsIsValid)
            await next(context);

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Session is no longer valid.");
        return;
    }
}
