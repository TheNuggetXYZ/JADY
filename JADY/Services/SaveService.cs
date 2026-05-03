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

    public SaveData SaveData { get; private set; } = new();
    public Config Config { get; private set; } = new();

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

            if (Config.AutoSave)
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
        SaveData.Diaries = diaries;

        _saveCoreService.Write(GetSavePath(), SaveData);

        UnsavedChanges = false;

        OnSave();
    }

    public void Save(Config config)
    {
        Config = config;
        Config.CultureInfo = new CultureInfo(config.CultureInfoName);

        _saveCoreService.Write(GetConfigPath(), Config);

        OnSave();

        if (UnsavedChanges && Config.AutoSave)
        {
            TriggerAutoSave();
        }
    }

    private void OnSave()
    {
        WeakReferenceMessenger.Default.Send(new Messages.SavePerformed());
        WeakReferenceMessenger.Default.Send(new Messages.JadySaveChanged());
    }

    public void Load()
    {
        SaveData = _saveCoreService.Read<SaveData>(GetSavePath());
        Config = _saveCoreService.Read<Config>(GetConfigPath());

        UnsavedChanges = false;

        _appVisualService.SetTheme(Config.IsThemeDark);

        WeakReferenceMessenger.Default.Send(new Messages.JadySaveChanged());
    }

    private string GetSavePath()
    {
        return Path.Combine(_saveCoreService.SavesDirectory, "JADY.save");
    }

    private string GetConfigPath()
    {
        return Path.Combine(_saveCoreService.SavesDirectory, "JADY.config");
    }
}
