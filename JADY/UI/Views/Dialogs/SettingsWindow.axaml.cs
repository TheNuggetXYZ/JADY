using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using JADY.Core.Data;
using JADY.Core.Models;
using JADY.Services;
using JADY.UI.Base;

namespace JADY.UI.Views.Dialogs;

public partial class SettingsWindow : DialogWindow<Config>
{
    private readonly ISaveService? _saveService;
    
    private readonly List<string> _systemFonts;

    // Required for the compiler and previewer
    public SettingsWindow()
    {
        InitializeComponent();
    }
    
    public SettingsWindow(ISaveService saveService) : this()
    {
        _saveService = saveService;

        ShowHidden.IsChecked = _saveService.Config.ShowHiddenEntries;
        AutoSave.IsChecked = _saveService.Config.AutoSave;
        SavePath.Text = saveService.SavesDirectory;
        AppTheme.ItemsSource = Enum.GetValues(typeof(AppTheme));
        AppTheme.SelectedIndex = (int)_saveService.Config.AppTheme;
        Cultures.ItemsSource = AppCultures.AvailableCultures;
        Cultures.SelectedItem = new CultureInfo(_saveService.Config.CultureInfoName);
        
        _systemFonts = FontManager.Current.SystemFonts
            .Select(fontFamily => fontFamily.Name)
            .OrderBy(name => name)
            .ToList();
        
        Fonts.ItemsSource = _systemFonts;

        var requestedFont = _systemFonts.FirstOrDefault(x => x == _saveService.Config.Font);

        Fonts.SelectedIndex = _systemFonts.IndexOf(requestedFont ?? "Noto Sans");
    }
    
    protected override Task SubmitAsync()
    {
        if (_saveService is null) 
            return Task.CompletedTask;
        
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
            Font = _systemFonts[Fonts.SelectedIndex]
        };
    }

    protected override InputElement? FocusedElement() => ShowHidden;

    private void Close_OnClick(object? sender, RoutedEventArgs e) => Close();
    private async void SaveClose_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
}