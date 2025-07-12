using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AdminByRequestChallenge.API;

public interface IJwtFactory
{
    string Create(UserInfo user);
}

public sealed class JwtFactory : IJwtFactory
{
    private readonly JwtOptions opt;
    private readonly SigningCredentials creds;

    public JwtFactory(IOptions<JwtOptions> options)
    {
        opt = options.Value;
        RSA rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(source: Convert.FromBase64String(opt.PrivateKey), bytesRead: out int _);
        creds = new SigningCredentials(new RsaSecurityKey(rsa),
            SecurityAlgorithms.RsaSha256);
    }

    public string Create(UserInfo user)
    {
        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            issuer: opt.Issuer,
            audience: opt.Audience,
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            },
            notBefore: now,
            expires: now.AddMinutes(opt.ExpiresMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}

public record JwtOptions
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string PrivateKey { get; init; } //RSA
    public int ExpiresMinutes { get; init; } = 60;
}