using System.Security.Cryptography;
using System.Text;
using VehicleInventory.Application.Abstractions;

namespace VehicleInventory.Infrastructure.Security;

/// <summary>
/// PBKDF2-SHA256 based password hasher.
/// Stores passwords as "base64(salt):base64(hash)" strings.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSize  = 16;
    private const int KeyLength = 32;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeyLength);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string storedHash)
    {
        var parts = storedHash.Split(':');
        if (parts.Length != 2) return false;

        byte[] salt;
        byte[] expectedHash;

        try
        {
            salt         = Convert.FromBase64String(parts[0]);
            expectedHash = Convert.FromBase64String(parts[1]);
        }
        catch
        {
            return false;
        }

        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeyLength);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
