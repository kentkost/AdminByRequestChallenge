using AdminByRequestChallange.Contracts;
using AdminByRequestChallenge.DataContext.Entities;
using System.Security.Claims;

namespace AdminByRequestChallenge.Core.Interfaces;

public interface IJwtProvider
{
    string CreateToken(string sessionKey, long expiration, User user, bool isGuest = false);
    List<Claim> GetBasicClaims(User user, string sessionKey, bool isGuest);
}