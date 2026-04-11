using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using JADY.Backend;
using JADY.Models;

namespace JADY.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();

        ShowHidden.IsChecked = DiaryJSON.JadySave.Settings.ShowHiddenEntries;
        SavePath.Text = DiaryJSON.JadySave.Settings.SaveFilePath;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (e.Key == Key.Escape)
            Close();
        else if (e is { Key: Key.Enter, KeyModifiers: KeyModifiers.Control })
        {
            SaveClose();
        }
    }

    private void ChangeSavePath_OnClick(object? sender, RoutedEventArgs e)
    {
        OpenChangeSavePath();
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

    private void Close_OnClick(object? sender, RoutedEventArgs e) => Close();
    private void SaveClose_OnClick(object? sender, RoutedEventArgs e) => SaveClose();
    private void Save_OnClick(object? sender, RoutedEventArgs e) => Save();
    
    /// <returns>
    /// Returns if the settings were saved.
    /// </returns>
    private async Task<bool> Save()
    {
        var saveFolder = await StorageProvider.TryGetFolderFromPathAsync(SavePath.Text);

        if (saveFolder == null)
        {
            WindowManager.OpenMessageBox(WindowManager.GetMainWindow(), "Warning",
                "The entered save file directory doesn't exist - resetting to last directory");
            SavePath.Text = DiaryJSON.JadySave.Settings.SaveFilePath;
            return false;
        }
        
        DiaryJSON.Save(new Settings()
        {
            ShowHiddenEntries = ShowHidden.IsChecked ?? false,
            SaveFilePath = SavePath.Text
        });
        return true;
    }


    private async Task SaveClose()
    {
        if (await Save())
            Close();
    }

}