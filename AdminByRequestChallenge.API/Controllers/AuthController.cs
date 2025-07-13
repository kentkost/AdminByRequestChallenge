using AdminByRequestChallenge.Core.Interfaces;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminByRequestChallenge.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IHttpContextAccessor context, IAuthService authService, IUsersService usersService) : ControllerBase
{
    //[Authorize UserFunctions.GuestAccessCreation]
    [HttpPost("CreateGuestAccess")]
    public async Task<ActionResult> CreateGuestAccess([FromBody] List<string> claims)
    {
        try
        {
            const string claimUsername = "Username";
            var con = context.HttpContext;
            var inviter = context.HttpContext.User.Claims.Where(x=> x.Type == claimUsername).Select(x => x.Value).First();
            var guestPassword = await usersService.CreateGuestUser(inviter, claims);
            return Ok(guestPassword);
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

    [HttpPost("Login"), AllowAnonymous]
    public async Task<IActionResult> NewLogin([FromBody] LoginDTO dto)
    {
        try
        {
            var newSession = await authService.CreateJwt(dto.Username, dto.Password);
            if (newSession != null)
                return Ok(newSession);

            return BadRequest();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("GuestLogin"), AllowAnonymous]
    public async Task<IActionResult> GuestLogin(string username, string password)
    {
        try
        {
            var newSession = await authService.CreateGuestJwt(username, password);
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

    public record LoginDTO(string Username, string Password);
}
