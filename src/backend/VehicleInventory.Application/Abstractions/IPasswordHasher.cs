namespace VehicleInventory.Application.Abstractions;

/// <summary>
/// Abstraction for password hashing and verification.
/// Implemented in the Infrastructure layer using PBKDF2.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain-text password, returning a storable "salt:hash" string.
    /// </summary>
    string Hash(string password);

    /// <summary>
    /// Verifies a plain-text password against a stored hash.
    /// </summary>
    bool Verify(string password, string storedHash);
}
