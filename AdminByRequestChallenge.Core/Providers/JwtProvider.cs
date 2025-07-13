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

    public Claim[] GetClaims(User user) => 
        new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

    string IJwtProvider.CreateToken(Session session, User user, Claim[] claims)
    {
        var jwt = new JwtSecurityToken(
                issuer: opt.Issuer,
                audience: opt.Audience,
                claims: claims,
                signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}