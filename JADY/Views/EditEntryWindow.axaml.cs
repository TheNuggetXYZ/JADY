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
        Close(new DiaryEntry()
        {
            Type = (DiaryEntryType)EntryType.SelectedIndex,
            Status = (DiaryEntryStatus)EntryStatus.SelectedIndex,
            Date = EntryDate.SelectedDate,
            EndDate = EntryEndDate.SelectedDate,
            Category = EntryCategory.Text,
            SubCategory = EntrySubcategory.Text,
            Title = EntryTitle.Text,
            Content = EntryContent.Text,
            IsHidden = EntryIsHidden.IsChecked ?? false,
        });
    }

    private void EntryType_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (EntryType == null || EntryStatus == null)
            return;
        
        if (EntryType.SelectedIndex == (int)DiaryEntryType.OneTime)
        {
            EntryStatus.SelectedIndex = (int)DiaryEntryStatus.None;
            EntryStatus.IsVisible = false;
        }
        else
        {
            if (EntryStatus.SelectedIndex == (int)DiaryEntryStatus.None)
            {
                EntryStatus.SelectedIndex = (int)DiaryEntryStatus.InProgress;
            }
            EntryStatus.IsVisible = true;
        }
    }

    private void EntryStatus_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (EntryType == null || EntryStatus == null)
            return;
        
        if (EntryStatus.SelectedIndex == (int)DiaryEntryStatus.None)
        {
            EntryType.SelectedIndex = (int)DiaryEntryType.OneTime;
        }
    }
}