using AdminByRequestChallange.Contracts;
using AdminByRequestChallenge.DataContext.Entities;
using System.Security.Claims;

namespace AdminByRequestChallenge.Core.Interfaces;

public interface IJwtProvider
{
    string CreateToken(Session session, User user, Claim[] claims);
    Claim[] GetClaims(User user);
}