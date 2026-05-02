using System.Globalization;
using System.IO;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Backend;
using JADY.Models;

namespace JADY.Services;

public class SaveService : ISaveService
{
    private readonly IAppVisualService _appVisualService;
    
    public JadySave JadySave { get; private set; } = new();

    public bool UnsavedChanges { get; private set; }

    public SaveService(IAppVisualService appVisualService)
    {
        _appVisualService = appVisualService;
        
        WeakReferenceMessenger.Default.Register<Messages.UnsavedChangeCreated>(this, (r, m) =>
        {
            UnsavedChanges = true;
            
            if (JadySave.Settings.AutoSave)
                TriggerAutoSave();
        });
    }

    private void TriggerAutoSave()
    {
        if (!UnsavedChanges) return;
        
        WeakReferenceMessenger.Default.Send(new Messages.PerformSave());
        
        UnsavedChanges = false;
    }

    public void Save(Diary[] diaries)
    {
        JadySave.Diaries = diaries;
        
        SaveFile();
    }

    public void Save(Settings settings)
    {
        JadySave.Settings = settings;
        JadySave.Settings.CultureInfo = new CultureInfo(settings.CultureInfoName);
        
        SaveFile();

        if (UnsavedChanges && JadySave.Settings.AutoSave)
        {
            TriggerAutoSave();
        }
    }

    private void SaveFile()
    {
        DiaryJSON.Serialize(GetSavePath(), JadySave);
        
        WeakReferenceMessenger.Default.Send(new Messages.SavePerformed());
        WeakReferenceMessenger.Default.Send(new Messages.JadySaveChanged());
    }

    public void Load()
    {
        JadySave = DiaryJSON.Deserialize(GetSavePath());
        JadySave.Load();

        _appVisualService.SetTheme(JadySave.Settings.IsThemeDark);

        WeakReferenceMessenger.Default.Send(new Messages.JadySaveChanged());
    }
    
    private string GetSavePath()
    {
        if (JadySave.Settings.SaveFilePath != null)
            return Path.Combine(JadySave.Settings.SaveFilePath, "JADY.save");
        
        throw new DirectoryNotFoundException("JADY save file path not found");
    }
}