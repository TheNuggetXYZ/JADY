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
    private List<CultureInfo> AvailableCultures { get; } = new()
    {
        new CultureInfo("cs-CZ"),
        new CultureInfo("en-US"),
        new CultureInfo("en-GB"),
        new CultureInfo("de-DE"),
        new CultureInfo("fr-FR"),
    };
    
    public SettingsWindow()
    {
        InitializeComponent();

        ShowHidden.IsChecked = Saves.JadySave.Settings.ShowHiddenEntries;
        AutoSave.IsChecked = Saves.JadySave.Settings.AutoSave;
        SavePath.Text = Saves.JadySave.Settings.SaveFilePath;
        Cultures.ItemsSource = AvailableCultures;
        Cultures.SelectedItem = new CultureInfo(Saves.JadySave.Settings.CultureInfoName);
    }
    
    protected override async Task TrySubmitAsync()
    {
        if (await CanSubmitAsync())
            await SubmitAsync();
        else
            await FixSavePath();
    }

    protected override Task SubmitAsync()
    {
        Saves.Save(GetValue());
        Close();
        return Task.CompletedTask;
    }

    protected override async Task<bool> CanSubmitAsync()
    {
        return await SavePathExists();
    }

    private async Task<bool> SavePathExists()
    {
        var saveFolder = await StorageProvider.TryGetFolderFromPathAsync(SavePath.Text);

        return saveFolder != null;
    }

    protected override Settings GetValue()
    {
        return new Settings()
        {
            ShowHiddenEntries = ShowHidden.IsChecked ?? false,
            AutoSave = AutoSave.IsChecked ?? false,
            SaveFilePath = SavePath.Text,
            CultureInfoName = AvailableCultures[Cultures.SelectedIndex].Name,
        };
    }

    protected override InputElement? GetFirstFocusableElementOverride() => ShowHidden;

    private async Task FixSavePath()
    {
        SavePath.Text = Saves.JadySave.Settings.SaveFilePath;
        
        await WindowManager.OpenMessageBox(WindowManager.GetMainWindow(), "Warning",
            "The entered save file directory doesn't exist - resetting to last directory");
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

        if (folders != null)
        {
            SavePath.Text = folders[0].Path.AbsolutePath;
        }
    }
    
    private async void ChangeSavePath_OnClick(object? sender, RoutedEventArgs e) => await OpenChangeSavePath();

    private void Close_OnClick(object? sender, RoutedEventArgs e) => Close();
    private async void SaveClose_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
}