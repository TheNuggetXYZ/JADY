using JADY.Core.Models;

namespace JADY.Services;

public interface ISaveCoreService
{
    string SavesDirectory { get; }
    
    void Write(string filePath, JadySave save);
    void Write(string savePath, Settings config);
    T Read<T>(string savePath) where T : new();
}