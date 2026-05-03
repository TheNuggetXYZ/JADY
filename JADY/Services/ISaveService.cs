using System.ComponentModel;
using JADY.Core.Models;

namespace JADY.Services;

public interface ISaveService : INotifyPropertyChanged
{
    SaveData SaveData { get; }
    Config Config { get; }
    bool UnsavedChanges { get; }
    string SavesDirectory { get; }
    
    void Save(Diary[] diaries);
    void Save(Config config);

    void Load();
}