using JADY.Core.Models;

namespace JADY.Services;

public interface IEncryptionService
{
    void StorePassword(string password, byte[] salt);
    
    EncryptedData Encrypt(string data);
    
    string Decrypt(EncryptedData encryptedData, out bool correctPassword);
    
    byte[] GenerateSalt();
}