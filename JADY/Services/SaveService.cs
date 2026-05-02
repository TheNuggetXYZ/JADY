using System.Globalization;
using System.IO;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Backend;
using JADY.Models;

namespace JADY.Services;

public class SaveService : ISaveService
{
    public JadySave JadySave { get; private set; } = new();

    public void Save(Diary[] diaries)
    {
        JadySave.Diaries = diaries;
        
        Save();
        
        WeakReferenceMessenger.Default.Send(new Messages.DiariesSavePerformed());
    }

    public void Save(Settings settings)
    {
        JadySave.Settings = settings;
        JadySave.Settings.CultureInfo = new CultureInfo(settings.CultureInfoName);
        
        Save();
    }
    
    private void Save()
    {
        DiaryJSON.Serialize(GetSavePath(), JadySave);
        
        WeakReferenceMessenger.Default.Send(new Messages.SavePerformed());
        WeakReferenceMessenger.Default.Send(new Messages.JadySaveChanged());
    }

    public void Load()
    {
        JadySave = DiaryJSON.Deserialize(GetSavePath());
        JadySave.Load();

        appVisualService.SetTheme(JadySave.Settings.IsThemeDark);

        WeakReferenceMessenger.Default.Send(new Messages.JadySaveChanged());
    }
    
    private string GetSavePath()
    {
        if (JadySave.Settings.SaveFilePath != null)
            return Path.Combine(JadySave.Settings.SaveFilePath, "JADY.save");
        
        throw new DirectoryNotFoundException("JADY save file path not found");
    }
}