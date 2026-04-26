using JADY.Models;

namespace JADY.Backend;

public interface ISaveService
{
    JadySave JadySave { get; }
    
    void Save(Diary[] diaries);
    void Save(Settings settings);

    void Load();
}