using System.Security.Cryptography;

namespace AdminByRequestChallenge.API;

public static class UserDatabase
{
    private const int _saltSize = 16; // 128 bits
    private const int _keySize = 32; // 256 bits
    private const int _iterations = 50000;
    private static readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA256;

    public static List<User> Users = new List<User>();

    public static User CreateDemoUser()
    {
        return CreateNewUser("Kent Kost", "me@kentkost.com", "password123");
    }

    public static User CreateNewUser(string name, string username, string password)
    {
        if (Users == null)
            Users = new List<User>();

        var salt = GenerateSalt();
        Guid guid = Guid.NewGuid();
        var user = new User()
        {
            ID = guid,
            Salt = salt,
            PasswordHash = HashPassword(password, salt),
            Email = username,
            Name = name,
        };

        Users.Add(user);
        return user;
    }

    public static bool VerifyPassword(string username, string password)
    {
        var user = Users.Where(x => x.Email == username).FirstOrDefault();

        if (user == null)
            return false;

        var pwHass = HashPassword(password, user.Salt);

        return Convert.ToBase64String(pwHass) == Convert.ToBase64String(user.PasswordHash);
    }

    private static byte[] GenerateSalt()
        => RandomNumberGenerator.GetBytes(_saltSize);

    private static byte[] HashPassword(string password, byte[] salt)
        => Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            _iterations,
            _algorithm,
            _keySize
        );
}

public class User
{
    public Guid ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = new byte[0];
    public byte[] Salt { get; set; } = new byte[0];
}

