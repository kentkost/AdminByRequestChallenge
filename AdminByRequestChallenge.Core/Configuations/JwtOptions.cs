﻿namespace AdminByRequestChallenge.Core.Configurations;

public class JwtOptions
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string PrivateKey { get; init; }
    public int ExpiresMinutes { get; init; } = 60;
}
