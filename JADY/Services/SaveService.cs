using System.Globalization;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Core;
using JADY.Core.Models;

namespace JADY.Services;

public partial class SaveService : ObservableObject, ISaveService
{
    private readonly ISaveCoreService _saveCoreService;
    private readonly IAppVisualService _appVisualService;
    
    public JadySave JadySave { get; private set; } = new();
    
    public string SavesDirectory => _saveCoreService.SavesDirectory;

    [ObservableProperty]
    private bool _unsavedChanges;

    public SaveService(ISaveCoreService saveCoreService, IAppVisualService appVisualService)
    {
        _saveCoreService = saveCoreService;
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
        
        UnsavedChanges = false;
        
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
        _saveCoreService.Write(GetMainSavePath(), JadySave);
        
        WeakReferenceMessenger.Default.Send(new Messages.SavePerformed());
        WeakReferenceMessenger.Default.Send(new Messages.JadySaveChanged());
    }

    public void Load()
    {
        JadySave = _saveCoreService.Read(GetMainSavePath());
        JadySave.Load();
        
        UnsavedChanges = false;

        _appVisualService.SetTheme(JadySave.Settings.IsThemeDark);

        WeakReferenceMessenger.Default.Send(new Messages.JadySaveChanged());
    }

    private string GetMainSavePath()
    {
        return Path.Combine(_saveCoreService.SavesDirectory, "JADY.save");
    }
}