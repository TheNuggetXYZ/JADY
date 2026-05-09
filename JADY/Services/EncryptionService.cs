using System;
using System.Security.Cryptography;
using System.Text;

namespace JADY.Services;

public class EncryptionService : IEncryptionService
{
    private byte[] _key = [];
    
    private const int KeyGenIterations = 200_000;
    private const int KeySize = 32;
    private const int SaltSize = 16;
    
    /// <summary>
    /// Call this when user enters a password
    /// </summary>
    public void StorePassword(string password, byte[] salt)
    {
        _key = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password), 
            salt, 
            KeyGenIterations, 
            HashAlgorithmName.SHA256, 
            KeySize);
    }

    /// <summary>Encrypts a string.</summary>
    /// <returns>encrypted data in the format of a byte array</returns>
    public byte[] Encrypt(string data)
    {
        if (_key.Length == 0)
            throw new InvalidOperationException("No key loaded.");
        
        // encrypt data with stored key
        throw new System.NotImplementedException();
    }
    
    /// <summary>Decrypts a byte array.</summary>
    /// <returns>decrypted data in the formate of a string.</returns>
    public string Decrypt(byte[] data)
    {
        if (_key.Length == 0)
            throw new InvalidOperationException("No key loaded.");
        
        // decrypt data with stored key
        throw new System.NotImplementedException();
    }

    public byte[] GenerateSalt() => RandomNumberGenerator.GetBytes(SaltSize);
}