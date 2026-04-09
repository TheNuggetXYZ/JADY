using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
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

    private void Save_OnClick(object? sender, RoutedEventArgs e)
    {
        DiaryJSON.Save(new Settings()
        {
            ShowHiddenEntries = ShowHidden.IsChecked ?? false,
            SaveFilePath = SavePath.Text
        });
    }
}