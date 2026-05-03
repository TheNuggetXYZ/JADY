using System.ComponentModel;
using JADY.Models;

namespace JADY.Services;

public interface ISaveService : INotifyPropertyChanged
{
    JadySave JadySave { get; }
    bool UnsavedChanges { get; }
    string SavesDirectory { get; }
    
    void Save(Diary[] diaries);
    void Save(Settings settings);

    void Load();
}