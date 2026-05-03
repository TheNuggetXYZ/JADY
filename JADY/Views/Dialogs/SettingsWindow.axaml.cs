using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Core.Models;
using JADY.Services;
using JADY.Views.Base;
using Microsoft.Extensions.Logging;

namespace JADY.Views.Dialogs;

public partial class SettingsWindow : DialogWindow<Config>
{
    private readonly IAppVisualService _appVisualService;
    private readonly ISaveService _saveService;
    private readonly ILogger<SettingsWindow> _logger;

    private List<CultureInfo> AvailableCultures { get; } = new()
    {
        new CultureInfo("cs-CZ"),
        new CultureInfo("en-US"),
        new CultureInfo("en-GB"),
        new CultureInfo("de-DE"),
        new CultureInfo("fr-FR"),
    };
    
    public SettingsWindow(IAppVisualService appVisualService, ISaveService saveService, ILogger<SettingsWindow> logger)
    {
        InitializeComponent();

        _appVisualService = appVisualService;
        _saveService = saveService;
        _logger = logger;

        ShowHidden.IsChecked = _saveService.Config.ShowHiddenEntries;
        AutoSave.IsChecked = _saveService.Config.AutoSave;
        DarkTheme.IsChecked = _saveService.Config.IsThemeDark;
        SavePath.Text = saveService.SavesDirectory;
        Cultures.ItemsSource = AvailableCultures;
        Cultures.SelectedItem = new CultureInfo(_saveService.Config.CultureInfoName);
    }
    
    protected override Task SubmitAsync()
    {
        UpdateApp();
        _saveService.Save(GetValue().Value);
        Close();
        return Task.CompletedTask;
    }

    private void UpdateApp()
    {
        bool newIsDark = DarkTheme.IsChecked ?? false;
        if (_saveService.Config.IsThemeDark != newIsDark)
        {
            _appVisualService.SetTheme(newIsDark);
        }
    }

    protected override Optional<Config> GetValue()
    {
        return new Config()
        {
            ShowHiddenEntries = ShowHidden.IsChecked ?? false,
            AutoSave = AutoSave.IsChecked ?? false,
            IsThemeDark = DarkTheme.IsChecked ?? false,
            CultureInfoName = AvailableCultures[Cultures.SelectedIndex].Name,
        };
    }

    protected override InputElement? FocusedElement() => ShowHidden;

    private void Close_OnClick(object? sender, RoutedEventArgs e) => Close();
    private async void SaveClose_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
}