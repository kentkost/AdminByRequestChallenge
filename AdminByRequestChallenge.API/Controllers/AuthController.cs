using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminByRequestChallenge.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    //[Authorize UserFunctions.GuestAccessCreation]
    [HttpPost("CreateGuestAccess"), AllowAnonymous]
    public async Task<ActionResult> CreateGuestAccess()
    {
        return Ok("Created one time password");
    }

    [HttpPost("Login"), AllowAnonymous]
    public async Task<ActionResult> CreateSession()
    {
        return Ok("Logged in");
    }

    [Authorize("AdminFunctions.Block")]
    [HttpPost("InvalidateSessions")]
    public async Task<ActionResult> InvalidateSessions(string username)
    {
        return Ok("Invalidated sessions");
    }
}
