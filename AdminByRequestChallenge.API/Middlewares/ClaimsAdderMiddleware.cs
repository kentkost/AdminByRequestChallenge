namespace AdminByRequestChallenge.API.Middlewares;

/// <summary>
/// The point of this middleware is to add the other Authorization Functions to the JWT.
/// </summary>
public class ClaimsAdderMiddleware
{
    private readonly RequestDelegate next;

    public ClaimsAdderMiddleware(RequestDelegate next)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext context, IHttpContextAccessor accessor)
    {
        await next(context);
    }
}
