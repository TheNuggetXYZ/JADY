using System;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Core.Data;
using JADY.Core.Models;
using JADY.Services;
using JADY.UI.Base;
using Microsoft.Extensions.Logging;

namespace JADY.UI.Views.Dialogs;

public partial class SettingsWindow : DialogWindow<Config>
{
    private readonly IAppVisualService _appVisualService;
    private readonly ISaveService _saveService;
    private readonly ILogger<SettingsWindow> _logger;
    
    public SettingsWindow(IAppVisualService appVisualService, ISaveService saveService, ILogger<SettingsWindow> logger)
    {
        InitializeComponent();

        _appVisualService = appVisualService;
        _saveService = saveService;
        _logger = logger;

        ShowHidden.IsChecked = _saveService.Config.ShowHiddenEntries;
        AutoSave.IsChecked = _saveService.Config.AutoSave;
        SavePath.Text = saveService.SavesDirectory;
        AppTheme.ItemsSource = Enum.GetValues(typeof(AppTheme));
        AppTheme.SelectedIndex = (int)_saveService.Config.AppTheme;
        Cultures.ItemsSource = AppCultures.AvailableCultures;
        Cultures.SelectedItem = new CultureInfo(_saveService.Config.CultureInfoName);
    }
    
    protected override Task SubmitAsync()
    {
        _saveService.Save(GetValue().Value);
        Close();
        return Task.CompletedTask;
    }

    protected override Optional<Config> GetValue()
    {
        return new Config()
        {
            ShowHiddenEntries = ShowHidden.IsChecked ?? false,
            AutoSave = AutoSave.IsChecked ?? false,
            AppTheme = (AppTheme)AppTheme.SelectedIndex,
            CultureInfoName = AppCultures.AvailableCultures[Cultures.SelectedIndex].Name,
        };
    }

    protected override InputElement? FocusedElement() => ShowHidden;

    private void Close_OnClick(object? sender, RoutedEventArgs e) => Close();
    private async void SaveClose_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
}