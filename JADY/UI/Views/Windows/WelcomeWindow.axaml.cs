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
    private readonly ISaveService _saveService;

    public WelcomeWindow(ISaveService saveService)
    {
        _saveService = saveService;
        
        InitializeComponent();
        
        AppTheme.ItemsSource = Enum.GetValues(typeof(AppTheme));
        AppTheme.SelectedIndex = (int)_saveService.Config.AppTheme;
        Cultures.ItemsSource = AppCultures.AvailableCultures;
        Cultures.SelectedItem = new CultureInfo(_saveService.Config.CultureInfoName);
    }

    private void ContinueButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _saveService.Config.AppTheme = (AppTheme)AppTheme.SelectedIndex;
        _saveService.Config.CultureInfoName = AppCultures.AvailableCultures[Cultures.SelectedIndex].Name;
        _saveService.Save(_saveService.Config);
        
        Close();
    }
}