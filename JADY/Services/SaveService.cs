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
    public Settings Settings { get; private set; } = new();

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

            if (Settings.AutoSave)
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

        _saveCoreService.Write(GetSavePath(), JadySave);

        UnsavedChanges = false;

        OnSave();
    }

    public void Save(Settings settings)
    {
        Settings = settings;
        Settings.CultureInfo = new CultureInfo(settings.CultureInfoName);

        _saveCoreService.Write(GetConfigPath(), Settings); // TODO: saving Settings in SaveCoreService

        OnSave();

        if (UnsavedChanges && Settings.AutoSave)
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
        JadySave = _saveCoreService.Read(GetSavePath());

        UnsavedChanges = false;

        _appVisualService.SetTheme(Settings.IsThemeDark);

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
