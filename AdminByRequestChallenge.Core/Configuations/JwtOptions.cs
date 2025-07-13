using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminByRequestChallenge.Core.Configurations;

public class JwtOptions
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string PrivateKey { get; init; } //RSA
    public int ExpiresMinutes { get; init; } = 60;
}
