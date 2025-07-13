using AdminByRequestChallange.Contracts;
using AdminByRequestChallenge.DataContext.Entities;
using System.Security.Claims;

namespace AdminByRequestChallenge.Core.Interfaces;

public interface IJwtProvider
{
    string CreateToken(Session session, User user, List<Claim> claims);
    string CreateToken(string sessionKey, long expiration, User user);
    List<Claim> GetBasicClaims(User user, string sessionKey);
}