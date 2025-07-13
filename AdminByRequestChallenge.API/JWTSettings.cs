namespace AdminByRequestChallange.API;

public class JWTSettings
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string PublicKey { get; init; }
    public int ExpiresMinutes { get; init; } = 60;
}