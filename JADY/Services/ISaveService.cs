using System.ComponentModel;
using System.Threading.Tasks;
using JADY.Core.Models;

namespace JADY.Services;

public interface ISaveService : INotifyPropertyChanged
{
    SaveFile SaveFile { get; }
    SaveData SaveData { get; }
    Config Config { get; }
    bool UnsavedChanges { get; }
    string SavesDirectory { get; }

    void SavePassword(string password);
    void Save(Diary[] diaries);
    void Save(Config config);

    Task LoadSave();
    void LoadConfig();

    bool ExistsConfig();
}