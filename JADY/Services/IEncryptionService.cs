using JADY.Core.Models;

namespace JADY.Services;

public interface IEncryptionService
{
    void StorePassword(string password, byte[] salt);
    
    byte[] Encrypt(string data);
    
    string Decrypt(byte[] data);
    
    byte[] GenerateSalt();
}