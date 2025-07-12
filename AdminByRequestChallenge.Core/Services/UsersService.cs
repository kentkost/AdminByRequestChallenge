using AdminByRequestChallenge.Contracts;
using AdminByRequestChallenge.Core.Interfaces;
using AdminByRequestChallenge.DataContext.Entities;
using System.Security.Cryptography;

namespace AdminByRequestChallenge.Core.Services;

public class UsersService(IUsersRepository repo) : IUsersService
{
    private const int saltSize = 16; // 128 bits
    private const int keySize = 32; // 256 bits
    private const int iterations = 12345;
    private static readonly HashAlgorithmName algo = HashAlgorithmName.SHA256;

    public async Task<bool> CreateUser(UserCreationDTO user)
    {
        var salt = GenerateSalt();
        var entUser = new User()
        {
            Username = user.Username,
            PasswordHash = HashPassword(user.Password, salt),
            Salt = salt
        };

        return await repo.CreateUser(entUser);
    }

    private static byte[] GenerateSalt() => RandomNumberGenerator.GetBytes(saltSize);

    private static byte[] HashPassword(string password, byte[] salt) 
        => Rfc2898DeriveBytes.Pbkdf2(password,salt, iterations, algo, keySize);
}
