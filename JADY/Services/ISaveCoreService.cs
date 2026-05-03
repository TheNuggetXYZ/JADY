using JADY.Core.Models;

namespace JADY.Services;

public interface ISaveCoreService
{
    string SavesDirectory { get; }
    
    void Write(string filePath, SaveData saveData);
    void Write(string savePath, Config config);
    T Read<T>(string savePath) where T : new();
}