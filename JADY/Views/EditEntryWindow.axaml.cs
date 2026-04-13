using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Models;

namespace JADY.Views;

public partial class EditEntryWindow : Window
{
    public EditEntryWindow()
    {
        InitializeComponent();

        EntryType.ItemsSource = Enum.GetValues(typeof(DiaryEntryType));
        EntryStatus.ItemsSource = Enum.GetValues(typeof(DiaryEntryStatus));
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (e.Key == Key.Escape)
            Close();
        else if (e is { Key: Key.Enter, KeyModifiers: KeyModifiers.Control })
        {
            Submit_OnClick(null, null);
        }
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