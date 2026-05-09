using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using JADY.Core.Models;

namespace JADY.Services;

public class EncryptionService : IEncryptionService
{
    private byte[] _key = [];
    
    private const int KeyGenIterations = 200_000;
    private const int KeySize = 32;
    private const int SaltSize = 16;
    private const int TagSize = 16;
    private const int NonceSize = 12;
    
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
    public EncryptedData Encrypt(string data)
    {
        if (_key.Length == 0)
            throw new InvalidOperationException("No key loaded.");
        
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);

        byte[] plainBytes = Encoding.UTF8.GetBytes(data);

        byte[] cipherBytes = new byte[plainBytes.Length];
        
        byte[] tag = new byte[TagSize];
        
        using var aes = new AesGcm(_key, TagSize);
        
        // Writes to cipherBytes and tag
        aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

        return new EncryptedData()
        {
            Data = cipherBytes,
            Nonce = nonce,
            Tag = tag,
        };
    }
    
    /// <summary>Decrypts a byte array.</summary>
    /// <returns>decrypted data in the formate of a string.</returns>
    public string Decrypt(EncryptedData encryptedData)
    {
        if (_key.Length == 0)
            throw new InvalidOperationException("No key loaded.");
        
        byte[] plainBytes = new byte[encryptedData.Data.Length];
        
        using var aes = new AesGcm(_key, TagSize);
        
        // Writes to plainBytes
        aes.Decrypt(encryptedData.Nonce, encryptedData.Data, encryptedData.Tag, plainBytes);
        
        return Encoding.UTF8.GetString(plainBytes);
    }

    public byte[] GenerateSalt() => RandomNumberGenerator.GetBytes(SaltSize);
}