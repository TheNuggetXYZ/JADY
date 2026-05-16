using System.Globalization;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Core;
using JADY.Core.Models;
using Microsoft.Extensions.Logging;

namespace JADY.Services;

public partial class SaveService : ObservableObject, ISaveService
{
    private readonly ILogger<SaveService> _logger;
    private readonly ISaveIoService _saveIoService;
    private readonly IAppVisualService _appVisualService;
    private readonly IEncryptionService _encryptionService;

    public SaveFile SaveFile { get; } = new();
    public SaveData SaveData { get; private set; } = new();
    public Config Config { get; private set; } = new();

    public string SavesDirectory => _saveIoService.SavesDirectory;

    [ObservableProperty]
    private bool _unsavedChanges;

    public SaveService(ILogger<SaveService> logger, ISaveIoService saveIoService, IAppVisualService appVisualService, IEncryptionService encryptionService)
    {
        _logger = logger;
        _saveIoService = saveIoService;
        _appVisualService = appVisualService;
        _encryptionService = encryptionService;

        WeakReferenceMessenger.Default.Register<Messages.UnsavedChangeCreated>(this, (r, m) =>
        {
            UnsavedChanges = true;

            if (Config.AutoSave)
                TriggerAutoSave();
        });
    }

    public void SavePassword(string password)
    {
        _logger.LogInformation("Saving password...");
        
        SaveFile.Salt ??= _encryptionService.GenerateSalt();

        _encryptionService.StorePassword(password, SaveFile.Salt);
    }

    public void Save(Diary[] diaries)
    {
        _logger.LogInformation("Saving main save...");
        
        SaveData.Diaries = diaries;

        _saveIoService.Write(GetSavePath(), SaveData, SaveFile);

        UnsavedChanges = false;

        OnSave();
    }

    public void Save(Config config)
    {
        _logger.LogInformation("Saving config...");
        
        Config = config;
        
        OnChangeConfig();

        _saveIoService.Write(GetConfigPath(), Config);

        OnSave();

        if (UnsavedChanges && Config.AutoSave)
        {
            TriggerAutoSave();
        }
    }

    public void LoadConfig()
    {
        _logger.LogInformation("Loading config...");
        
        Config = _saveIoService.ReadConfig(GetConfigPath());
        
        OnChangeConfig();
    }

    public async Task LoadSave()
    {
        _logger.LogInformation("Loading main save...");

        var result = _saveIoService.ReadSave(GetSavePath());
        await ResolveLoadResult(result);

        UnsavedChanges = false;
        
        WeakReferenceMessenger.Default.Send(new Messages.JadySaveChanged());
    }

    private async Task ResolveLoadResult(SaveIoService.LoadResult loadResult)
    {
        switch (loadResult.Status)
        {
            case LoadStatus.Success:
                SaveData = loadResult.Data ?? throw new InvalidOperationException("LoadResult.Data should not be null while LoadResult.Status is Success");
                SaveFile = loadResult.Container ?? throw new InvalidOperationException("LoadResult.Container should not be null while LoadResult.Status is Success");
                break;

            case LoadStatus.InvalidPassword:
                SaveData = new SaveData();
                SaveFile = loadResult.Container ?? throw new InvalidOperationException("LoadResult.Container should not be null while LoadResult.Status is InvalidPassword");

                Optional<string?> result;

                while (true)
                {
                    result = await _windowService.OpenDialogWindowDI<LoginWindow, string?>(_windowService.GetMainWindow());
                    
                    if (result.HasValue && !string.IsNullOrWhiteSpace(result.Value))
                        break;
                }

                if (SaveFile.Salt is null)
                    throw new InvalidOperationException("SaveFile.Salt is null and the save is encrypted, cannot login and load save");
                
                _encryptionService.StorePassword(result.Value, SaveFile.Salt);

                await LoadSave();
                
                break;
            
            case LoadStatus.Corrupted: // TODO: corrupted save window
                break;
            
            case LoadStatus.FileNotFound:
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(loadResult), loadResult.Status, null);
        }
    }

    public bool ExistsConfig()
    {
        return _saveIoService.ExistsFile(GetConfigPath());
    }

    private void OnSave()
    {
        WeakReferenceMessenger.Default.Send(new Messages.SavePerformed());
        WeakReferenceMessenger.Default.Send(new Messages.JadySaveChanged());
    }

    private void OnChangeConfig()
    {
        _appVisualService.SetTheme(Config.AppTheme);
    }

    private void TriggerAutoSave()
    {
        if (!UnsavedChanges) return;

        WeakReferenceMessenger.Default.Send(new Messages.PerformSave());

        UnsavedChanges = false;
    }

    private string GetSavePath()
    {
        return Path.Combine(_saveIoService.SavesDirectory, "JADY.save");
    }

    private string GetConfigPath()
    {
        return Path.Combine(_saveIoService.SavesDirectory, "JADY.config");
    }
}
