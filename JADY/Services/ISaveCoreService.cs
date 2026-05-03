using JADY.Core.Models;

namespace JADY.Services;

public interface ISaveCoreService
{
    string SavesDirectory { get; }
    
    void Write(string filePath, JadySave save);
    JadySave Read(string filePath);
}