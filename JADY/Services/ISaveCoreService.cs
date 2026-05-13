using JADY.Core.Models;

namespace JADY.Services;

public interface ISaveCoreService
{
    SaveFile SaveFile { get; }
    string SavesDirectory { get; }
    
    void Write(string path, SaveData saveData);
    void Write(string path, Config config);

    Config ReadConfig(string path);
    SaveData ReadSave(string path);

    bool ExistsFile(string path);
}