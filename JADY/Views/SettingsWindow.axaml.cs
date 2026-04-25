using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using JADY.Backend;
using JADY.Models;

namespace JADY.Views;

public partial class SettingsWindow : DialogWindowBase<Settings>
{
    private AppVisualService _appVisualService;
    
    private List<CultureInfo> AvailableCultures { get; } = new()
    {
        new CultureInfo("cs-CZ"),
        new CultureInfo("en-US"),
        new CultureInfo("en-GB"),
        new CultureInfo("de-DE"),
        new CultureInfo("fr-FR"),
    };
    
    public SettingsWindow(AppVisualService appVisualService)
    {
        InitializeComponent();

        _appVisualService = appVisualService;
        
        ShowHidden.IsChecked = Saves.JadySave.Settings.ShowHiddenEntries;
        AutoSave.IsChecked = Saves.JadySave.Settings.AutoSave;
        DarkTheme.IsChecked = Saves.JadySave.Settings.IsThemeDark;
        SavePath.Text = Saves.JadySave.Settings.SaveFilePath;
        Cultures.ItemsSource = AvailableCultures;
        Cultures.SelectedItem = new CultureInfo(Saves.JadySave.Settings.CultureInfoName);
    }
    
    protected override async Task TrySubmitAsync()
    {
        if (await CanSubmitAsync())
        {
            await SubmitAsync();
        }
        else
        {
            if (!await SavePathValid())
                await FixSavePath();
        }
    }

    protected override Task SubmitAsync()
    {
        UpdateApp();
        Saves.Save(GetValue());
        Close();
        return Task.CompletedTask;
    }

    protected override async Task<bool> CanSubmitAsync()
    {
        return await SavePathValid();
    }

    private async Task<bool> SavePathValid()
    {
        var saveFolder = await StorageProvider.TryGetFolderFromPathAsync(SavePath.Text);

        try
        {
            // Cleanup the path and partially fix it. E.g.: When you have "home/user\" it will still get the folder and incorrectly flag the path as valid. Then it will try to save to "home/user\/JADY.save"
            SavePath.Text = saveFolder?.Path.AbsolutePath;
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine($"Cannot set save path to the path of the found folder. Exception: {e}");
            return false;
        }

        return saveFolder != null && Utils.IsDirectoryWritable(SavePath.Text);
    }

    private void UpdateApp()
    {
        bool newIsDark = DarkTheme.IsChecked ?? false;
        if (Saves.JadySave.Settings.IsThemeDark != newIsDark)
        {
            _appVisualService.SetTheme(newIsDark);
        }
    }

    protected override Settings GetValue()
    {
        return new Settings()
        {
            ShowHiddenEntries = ShowHidden.IsChecked ?? false,
            AutoSave = AutoSave.IsChecked ?? false,
            IsThemeDark = DarkTheme.IsChecked ?? false,
            SaveFilePath = SavePath.Text,
            CultureInfoName = AvailableCultures[Cultures.SelectedIndex].Name,
        };
    }

    protected override InputElement? GetFirstFocusableElementOverride() => ShowHidden;

    private async Task FixSavePath()
    {
        SavePath.Text = Saves.JadySave.Settings.SaveFilePath;

        await WindowManager.OpenMessageBox(WindowManager.GetMainWindow(), "Warning",
            "The entered save file directory is invalid - resetting to last directory");
    }

    private async Task OpenChangeSavePath()
    {
        IStorageFolder? suggestedStart =
            await StorageProvider.TryGetFolderFromPathAsync(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Choose save location",
            SuggestedStartLocation = suggestedStart
        });

        if (folders is { Count: > 0 })
        {
            SavePath.Text = folders[0].Path.AbsolutePath;
        }
    }
    
    private async void ChangeSavePath_OnClick(object? sender, RoutedEventArgs e) => await OpenChangeSavePath();

    private void Close_OnClick(object? sender, RoutedEventArgs e) => Close();
    private async void SaveClose_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
}