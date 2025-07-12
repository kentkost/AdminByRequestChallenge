using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminByRequestChallenge.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(ILogger<AuthController> logger, IHttpContextAccessor context, ISessionStore store) : ControllerBase
{
    //[Authorize UserFunctions.GuestAccessCreation]
    [HttpPost("CreateGuestAccess"), AllowAnonymous]
    public async Task<ActionResult> CreateGuestAccess()
    {
        return Ok("Created one time password");
    }

    [HttpPost("CreateSession"), AllowAnonymous]
    public async Task<ActionResult> CreateSession([FromBody] LoginRequest loginRequest)
    {
        var session = SessionDatabase.CreateSession(loginRequest.Username, loginRequest.Password);

        var siteRestrictions = SameSiteMode.None;
        if (session != null && context.HttpContext != null)
        {
            context.HttpContext.Response.Cookies.Append("SessionID", session.SessionID, new CookieOptions
            {
                HttpOnly = true, 
                Secure = true, 
                SameSite = siteRestrictions,
                Expires = session.Expiration
            });

            context.HttpContext.Response.Cookies.Append("Username", session.UserName, new CookieOptions
            {
                HttpOnly = true, 
                Secure = true, 
                SameSite = siteRestrictions,
                Expires = session.Expiration
            });
            return Ok("Logged in");
        }

        return BadRequest("Couldn't log in");
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        try
        {
            var key = store.CreateSession(dto.Username, dto.Password);
            return Ok(new { session_key = key });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }

    }
    public record LoginDto(string Username, string Password);


    [Authorize("AdminFunctions.Block")]
    [HttpPost("InvalidateSessions")]
    public async Task<ActionResult> InvalidateSessions(string username)
    {
        return Ok("Invalidated sessions");
    }
}
