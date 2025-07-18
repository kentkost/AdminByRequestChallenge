﻿using AdminByRequestChallenge.Contracts;

namespace AdminByRequestChallenge.Core.Interfaces;

public interface IUsersService
{
    Task<bool> CreateUser(UserCreationDTO user);
    Task<string> CreateGuestUser(string inviter, List<string> claims);
}