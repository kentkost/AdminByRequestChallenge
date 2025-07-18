﻿using AdminByRequestChallenge.Contracts;
using AdminByRequestChallenge.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminByRequestChallenge.API.Controllers;

#if DEBUG
// This just exists for some manual testing
[ApiController]
[Route("[controller]")]
public class DeveloperController(IUsersService usersService) : ControllerBase
{

    [HttpPost("CreateUser"), AllowAnonymous]
    public async Task<ActionResult> CreateUser([FromBody] UserCreationDTO newUser)
    {
        var res = await usersService.CreateUser(newUser);
        
        if(res)
            return Ok("Created User");

        return BadRequest("Something unexpected happened");
    }

    [HttpGet("InitializeTestData"), AllowAnonymous]
    public async Task<ActionResult> InitializeTestData()
    {
        var res = await usersService.CreateUser(new UserCreationDTO() { Username ="testuser", Password="notSoSecret"});

        return Ok();
    }
}
#endif
