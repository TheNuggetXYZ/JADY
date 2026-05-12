using System;
using System.Security.Cryptography;
using System.Text;
using JADY.Core.Models;
using Microsoft.Extensions.Logging;

namespace JADY.Services;

public class EncryptionService(ILogger<EncryptionService> logger) : IEncryptionService
{
    private byte[] _key = [];

    private bool _hasPassword;
    
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
        logger.LogInformation("Storing password...");
        
        _key = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password), 
            salt, 
            KeyGenIterations, 
            HashAlgorithmName.SHA256, 
            KeySize);
        
        _hasPassword = true;
    }

    /// <summary>Encrypts a string.</summary>
    /// <returns>encrypted data in the format of a byte array if a password was entered, otherwise returns unencrypted byte array</returns>
    public EncryptedData Encrypt(string data)
    {
        logger.LogInformation("Encrypting data...");
        
        if (!_hasPassword)
        {
            logger.LogInformation("No password found, skipping encryption");
            
            return new EncryptedData()
            {
                Encrypted = false,
                Data = Encoding.UTF8.GetBytes(data),
                Nonce = [],
                Tag = [],
            };
        }
        
        if (_key.Length == 0)
            throw new InvalidOperationException("No key loaded");
        
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);

        byte[] plainBytes = Encoding.UTF8.GetBytes(data);

        byte[] cipherBytes = new byte[plainBytes.Length];
        
        byte[] tag = new byte[TagSize];
        
        using var aes = new AesGcm(_key, TagSize);
        
        // Writes to cipherBytes and tag
        aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

        return new EncryptedData()
        {
            Encrypted = true,
            Data = cipherBytes,
            Nonce = nonce,
            Tag = tag,
        };
    }
    
    /// <summary>Decrypts a byte array if it is encrypted.</summary>
    /// <returns>decrypted data in the format of a string.</returns>
    public string Decrypt(EncryptedData encryptedData, out bool correctPassword)
    {
        correctPassword = true;
        
        logger.LogInformation("Decrypting data...");
        
        if (!encryptedData.Encrypted)
        {
            logger.LogInformation("Data is not encrypted, skipping decryption");
            
            return Encoding.UTF8.GetString(encryptedData.Data);
        }
        
        if (_key.Length == 0)
            throw new InvalidOperationException("No key loaded");
        
        byte[] plainBytes = new byte[encryptedData.Data.Length];
        
        using var aes = new AesGcm(_key, TagSize);
        
        try
        {
            // Writes to plainBytes
            aes.Decrypt(encryptedData.Nonce, encryptedData.Data, encryptedData.Tag, plainBytes);
        }
        catch (AuthenticationTagMismatchException)
        {
            logger.LogWarning("Incorrect password");
            
            correctPassword = false;
            return string.Empty;
        }
        
        return Encoding.UTF8.GetString(plainBytes);
    }

    public byte[] GenerateSalt() => RandomNumberGenerator.GetBytes(SaltSize);
}