using System.ComponentModel;
using JADY.Core.Models;

namespace JADY.Services;

public interface ISaveService : INotifyPropertyChanged
{
    JadySave JadySave { get; }
    Settings Settings { get; }
    bool UnsavedChanges { get; }
    string SavesDirectory { get; }
    
    void Save(Diary[] diaries);
    void Save(Settings settings);

    void Load();
}