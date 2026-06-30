using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Core;
using JADY.Core.Data;
using JADY.Core.Models;
using JADY.UI.Views.Dialogs;
using Microsoft.Extensions.Logging;

namespace JADY.Services;

public partial class SaveService : ObservableObject, ISaveService
{
    private readonly ILogger<SaveService> _logger;
    private readonly ISaveIoService _saveIoService;
    private readonly IAppVisualService _appVisualService;
    private readonly IEncryptionService _encryptionService;
    private readonly IWindowService _windowService;

    public SaveFile SaveFile { get; private set; } = new();
    public SaveData SaveData { get; private set; } = new();
    public Config Config { get; private set; } = new();

    public string SavesDirectory => _saveIoService.SavesDirectory;

    [ObservableProperty]
    private bool _unsavedChanges;

    public SaveService(ILogger<SaveService> logger, ISaveIoService saveIoService, IAppVisualService appVisualService, IEncryptionService encryptionService, IWindowService windowService)
    {
        _logger = logger;
        _saveIoService = saveIoService;
        _appVisualService = appVisualService;
        _encryptionService = encryptionService;
        _windowService = windowService;

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

        await LoadSaveContainer_ExtractSaveData();

        UnsavedChanges = false;
        
        WeakReferenceMessenger.Default.Send(new Messages.JadySaveChanged());
    }

    private async Task LoadSaveContainer_ExtractSaveData()
    {
        var container = _saveIoService.ReadSaveContainer(GetSavePath());

        switch (container.Status)
        {
            case SaveIoService.ReadStatus.Success:
                SaveFile = container.Data ?? throw new InvalidOperationException(
                    "LoadResult.Data should not be null while LoadResult.Status is Success");

                await LoadSaveFromContainer();
                break;
            
            case SaveIoService.ReadStatus.Corrupted:
                if (container.Data is not null)
                {
                    _windowService.OpenMessageBox("Your save file is corrupted, but a backup was found. This is likely due to tampering with the save.", "Corrupted save");
                    
                    SaveFile = container.Data;
                    await LoadSaveFromContainer();
                }
                else
                {
                    _windowService.OpenMessageBox("Your save file is corrupted and a backup was not found. This is likely due to tampering with the save.", "Corrupted save");
                }
                    
                
                break;
            
            case SaveIoService.ReadStatus.FileNotFound:
                _windowService.OpenMessageBox("No save found. New save initialized.", "No save found");
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(container), container.Status, null);
        }
    }

    private async Task LoadSaveFromContainer()
    {
        while (true)
        {
            var save = _saveIoService.ReadSaveFromContainer(SaveFile);

            switch (save.Status)
            {
                case LoadStatus.Success:
                    SaveData = save.Data ?? throw new InvalidOperationException("LoadResult.Data should not be null while LoadResult.Status is Success");
                    break;
                
                case LoadStatus.InvalidPassword:
                {
                    IClassicDesktopStyleApplicationLifetime desktop;
                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime _desktop)
                        desktop = _desktop;
                    else
                        throw new InvalidOperationException("Invalid ApplicationLifetime");

                    string password;

                    while (true)
                    {
                        var loginWindow = new LoginWindow();
                        desktop.MainWindow = loginWindow;
                        desktop.MainWindow.Show();

                        TaskCompletionSource<bool> tcs = new();
                        loginWindow.Closed += (_, __) => tcs.TrySetResult(true);
                        await tcs.Task;
                        
                        var wasPasswordEntered = loginWindow.EnteredPassword;
                        var rawPassword = loginWindow.RawPassword;
                        
                        if (!wasPasswordEntered) // this is true when you explicitly close the login window
                            desktop.Shutdown(0);

                        if (wasPasswordEntered && !string.IsNullOrWhiteSpace(rawPassword))
                        {
                            password = rawPassword;
                            break;
                        }
                    }

                    if (SaveFile.Salt is null) throw new InvalidOperationException("SaveFile.Salt is null and the save is encrypted, cannot login and load save");

                    _encryptionService.StorePassword(password, SaveFile.Salt);

                    continue;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(save.Status), save.Status, null);
            }

            break;
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
        _appVisualService.SetFont(Config.Font);

        CultureInfo.CurrentCulture = Config.CultureInfo;
        CultureInfo.CurrentUICulture = Config.CultureInfo;
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
