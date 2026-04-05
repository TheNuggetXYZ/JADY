using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using JADY.Backend;
using JADY.Models;
using JADY.ViewModels;

namespace JADY.Views;

public partial class EditEntryWindow : Window
{
    public EditEntryWindow()
    {
        InitializeComponent();

        EntryType.ItemsSource = Enum.GetValues(typeof(DiaryEntryType));
        EntryStatus.ItemsSource = Enum.GetValues(typeof(DiaryEntryStatus));
    }

    private void Submit_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }
}