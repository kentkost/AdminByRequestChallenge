using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AdminByRequestChallenge.API;

public sealed class SessionKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "SessionKey";
    public string HeaderName { get; init; } = "X-Session-Key";
}

public sealed class SessionKeyAuthenticationHandler
    : AuthenticationHandler<SessionKeyAuthenticationOptions>
{
    private readonly ISessionStore _store;
    private readonly IJwtFactory _jwtFactory;

    public SessionKeyAuthenticationHandler(
        IOptionsMonitor<SessionKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISessionStore store,
        IJwtFactory jwtFactory)
        : base(options, logger, encoder)
    {
        _store = store;
        _jwtFactory = jwtFactory;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var values))
            return Task.FromResult(AuthenticateResult.NoResult());

        var key = values.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(key))
            return Task.FromResult(AuthenticateResult.NoResult());

        // 2) validate
        var user = _store.GetUser(key!);
        if (user is null)
            return Task.FromResult(AuthenticateResult.Fail("Invalid session key"));

        // 3) build principal
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);

        // 4) mint JWT & stick it somewhere handy
        var jwt = _jwtFactory.Create(user);
        //   a) make it a claim so downstream code can read it easily
        identity.AddClaim(new Claim("access_token", jwt));
        //   b) (optional) make it available via HttpContext.Items
        Context.Items["access_token"] = jwt;

        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
