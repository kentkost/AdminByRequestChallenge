using AdminByRequestChallenge.Core.Configurations;
using AdminByRequestChallenge.Core.Interfaces;
using AdminByRequestChallenge.DataContext.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AdminByRequestChallenge.Providers;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions opt;
    private readonly SigningCredentials creds;

    public JwtProvider(JwtOptions options)
    {
        opt = options;
        RSA rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(source: Convert.FromBase64String(opt.PrivateKey), bytesRead: out int _);
        creds = new SigningCredentials(new RsaSecurityKey(rsa),
            SecurityAlgorithms.RsaSha256);
    }

    public string CreateToken(string sessionKey, long expiration, User user)    
    {
        //IEnumerable<int> roleFunctions = new List<int> { 1, 2, 3, 4 };
        var claims = GetBasicClaims(user, sessionKey);
        //claims.AddRange(roleFunctions.Select(rf => new Claim("AF", rf.ToString())));
        
        var jwt = new JwtSecurityToken(
                issuer: opt.Issuer,
                audience: opt.Audience,
                claims: claims,
                expires: new DateTime(expiration, DateTimeKind.Utc),
                signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public List<Claim> GetBasicClaims(User user, string sessionKey) =>
        new List<Claim>() {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("Username", user.Username),
            new Claim("SessionKey", sessionKey)
        };

    public string CreateToken(Session session, User user, List<Claim> claims)
    {
        var jwt = new JwtSecurityToken(
                issuer: opt.Issuer,
                audience: opt.Audience,
                claims: claims,
                signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}