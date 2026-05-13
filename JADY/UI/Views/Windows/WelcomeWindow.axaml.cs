using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using JADY.Core.Data;
using JADY.Core.Models;
using JADY.Services;

namespace JADY.UI.Views.Windows;

public partial class WelcomeWindow : Window
{
    private readonly ISaveService? _saveService;
    private readonly IWindowService _windowService;

    // Required for the compiler and previewer
    public WelcomeWindow()
    {
        InitializeComponent();
    }
    
    public WelcomeWindow(ISaveService saveService, IWindowService windowService) : this()
    {
        _saveService = saveService;
        _windowService = windowService;

        AppTheme.ItemsSource = Enum.GetValues(typeof(AppTheme));
        AppTheme.SelectedIndex = (int)_saveService.Config.AppTheme;
        Cultures.ItemsSource = AppCultures.AvailableCultures;
        Cultures.SelectedItem = new CultureInfo(_saveService.Config.CultureInfoName);
    }

    private void ContinueButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_saveService is null) return;

        if (!string.IsNullOrWhiteSpace(Password.Text) && Password.Text != ConfirmPassword.Text)
        {
            _windowService.OpenMessageBox("Passwords do not match!");
            return;
        }
        
        _saveService.Config.AppTheme = (AppTheme)AppTheme.SelectedIndex;
        _saveService.Config.CultureInfoName = AppCultures.AvailableCultures[Cultures.SelectedIndex].Name;
        _saveService.Save(_saveService.Config);

        Close();
    }
}