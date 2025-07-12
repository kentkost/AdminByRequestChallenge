using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminByRequestChallenge.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IHttpContextAccessor context, ISessionStore sessionStore) : ControllerBase
{
    //[Authorize UserFunctions.GuestAccessCreation]
    [HttpPost("CreateGuestAccess"), AllowAnonymous]
    public async Task<ActionResult> CreateGuestAccess(List<string> claims)
    {
        return Ok("Created one time password");
    }

    /// TODO: Replace sessions with JWT instead. And then just populate JWT with audiences and claims
    [HttpPost("Login"), AllowAnonymous]
    public IActionResult Login([FromBody] LoginDTO dto)
    {
        try
        {
            var newSession = sessionStore.CreateSession(dto.Username, dto.Password);
            return Ok(newSession);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }

    }

    [Authorize("AdminFunctions.Block")]
    [HttpPost("InvalidateSessions")]
    public async Task<ActionResult> InvalidateSessions(string username)
    {
        return Ok("Invalidated sessions");
    }


    [HttpPost("CreateSession"), AllowAnonymous, Obsolete("Abandoned concept", true)]
    public async Task<ActionResult> CreateSession([FromBody] LoginDTO loginRequest)
    {
        var session = sessionStore.CreateSession(loginRequest.Username, loginRequest.Password);

        var siteRestrictions = SameSiteMode.None;
        if (session != null && context.HttpContext != null)
        {
            context.HttpContext.Response.Cookies.Append("SessionID", session.session, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = siteRestrictions,
                Expires = DateTimeOffset.FromUnixTimeSeconds(session.expiration),
            });

            return Ok("Logged in");
        }

        return BadRequest("Couldn't log in");
    }

    public record LoginDTO(string Username, string Password);
}
