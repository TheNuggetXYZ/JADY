using JADY.Models;

namespace JADY.Services;

public interface ISaveService
{
    JadySave JadySave { get; }
    
    bool UnsavedChanges { get; }
    
    void Save(Diary[] diaries);
    void Save(Settings settings);

    void Load();
}