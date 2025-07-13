using AdminByRequestChallenge.Core.Interfaces;
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
    private readonly ISessionRepository sessionRepo;
    private readonly IJwtProvider jwtProvider;
    private readonly IUserRepository userRepo;

    public SessionKeyAuthenticationHandler(
        IOptionsMonitor<SessionKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISessionRepository sessionRepo,
        IJwtProvider jwtProvider,
        IUserRepository userRepo)
        : base(options, logger, encoder)
    {
        this.sessionRepo = sessionRepo;
        this.jwtProvider = jwtProvider;
        this.userRepo = userRepo;
    }

    // God damn it I hate this soooooo much. It's ugly. It's expensive. And somewhat stupid.
    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var values))
            return AuthenticateResult.NoResult();

        var key = values.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(key))
            return AuthenticateResult.NoResult();
        

        var session = await sessionRepo.GetNonExpiredSession(key);
        if (session is null)
            return AuthenticateResult.Fail("Invalid or expired session");

        if(session.IsGuest && session.HasBeenUsed)
            return AuthenticateResult.Fail("Guest session already been used");
        
        var user = await userRepo.GetUser(session.Username);
        var claims = jwtProvider.GetClaims(user);

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);

        // 4) mint JWT & stick it somewhere handy
        var jwt = jwtProvider.CreateToken(session, user, claims);
        //   a) make it a claim so downstream code can read it easily
        identity.AddClaim(new Claim("access_token", jwt));
        //   b) (optional) make it available via HttpContext.Items
        Context.Items["access_token"] = jwt;
        sessionRepo.SetSessionAsUsed(key);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
