using System.Security.Cryptography;

namespace AdminByRequestChallenge.Core.Providers;

public static class PasswordHashProvider
{
    private const int saltSize = 16; // 128 bits
    private const int keySize = 32; // 256 bits
    private const int iterations = 12345;
    private static readonly HashAlgorithmName algo = HashAlgorithmName.SHA256;

    public static byte[] GenerateSalt() => RandomNumberGenerator.GetBytes(saltSize);

    public static byte[] HashPassword(string password, byte[] salt)
        => Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, algo, keySize);

    public static string GenerateSixDigitCode()
    {
        Span<byte> buffer = stackalloc byte[4];
        RandomNumberGenerator.Fill(buffer);
        uint randomInt = BitConverter.ToUInt32(buffer) & 0x7FFFFFFF;
        return (randomInt % 1_000_000).ToString("D6");
    }
}
