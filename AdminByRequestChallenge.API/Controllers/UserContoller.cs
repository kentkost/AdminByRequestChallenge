using AdminByRequestChallenge.Contracts;
using AdminByRequestChallenge.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminByRequestChallenge.API.Controllers;

#if DEBUG
// This just exists for some manual testing
[ApiController]
[Route("[controller]")]
public class UserContoller(IUsersService usersService) : ControllerBase
{

    [HttpPost, AllowAnonymous]
    public async Task<ActionResult> CreateUser([FromBody] UserCreationDTO newUser)
    {
        var res = await usersService.CreateUser(newUser);
        
        if(res)
            return Ok("Created User");

        return BadRequest("Something unexpected happened");
    }
}
#endif
