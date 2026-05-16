using JADY.Core.Models;

namespace JADY.Services;

public interface ISaveIoService
{
    string SavesDirectory { get; }
    
    void Write(string path, SaveData saveData, SaveFile saveFile);
    void Write(string path, Config config);

    Config ReadConfig(string path);
    SaveIoService.LoadResult ReadSave(string path);
    SaveIoService.LoadResult ReadSaveContainer(string path);

    bool ExistsFile(string path);
}