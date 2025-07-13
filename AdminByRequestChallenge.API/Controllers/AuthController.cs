using AdminByRequestChallenge.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminByRequestChallenge.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IHttpContextAccessor context, IAuthService authService) : ControllerBase
{
    //[Authorize UserFunctions.GuestAccessCreation]
    [HttpPost("CreateGuestAccess")]
    public async Task<ActionResult> CreateGuestAccess([FromBody] List<string> claims)
    {
        try
        {
            const string claimUsername = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
            var con = context.HttpContext;
            var inviter = context.HttpContext.User.Claims.Where(x=> x.Type == claimUsername).Select(x => x.Value).First();
            var guestSession = await authService.CreateGuestSession(inviter, claims);
            return Ok(guestSession);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch
        {
            return BadRequest("Couldn't create guest session");
        }
    }

    /// TODO: Replace sessions with JWT instead. And then just populate JWT with audiences and claims
    [HttpPost("Login"), AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        try
        {
            var newSession = await authService.CreateSession(dto.Username, dto.Password);
            if (newSession != null)
                return Ok(newSession);
            
            return BadRequest();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("InvalidateSessions"), Authorize("AdminFunctions.InvalidateSessions")]
    public async Task<ActionResult> InvalidateSessions(string username)
    {
        throw new NotImplementedException();
        return Ok("Invalidated sessions");
    }


    //[HttpPost("CreateSession"), AllowAnonymous, Obsolete("Abandoned concept. Had an idea, but leaving it as a talking point", true)]
    //public async Task<ActionResult> CreateSession([FromBody] LoginDTO loginRequest)
    //{
    //    throw new Exception("Obsolete.");
    //    var newSession = sessionStore.CreateSession(loginRequest.Username, loginRequest.Password);

    //    var siteRestrictions = SameSiteMode.None;
    //    if (newSession != null && context.HttpContext != null)
    //    {
    //        context.HttpContext.Response.Cookies.Append("SessionID", newSession.SessionKey, new CookieOptions
    //        {
    //            HttpOnly = true,
    //            Secure = true,
    //            SameSite = siteRestrictions,
    //            Expires = DateTimeOffset.FromUnixTimeSeconds(newSession.Expiration),
    //        });

    //        return Ok("Logged in");
    //    }

    //    return BadRequest("Couldn't log in");
    //}

    public record LoginDTO(string Username, string Password);
}
