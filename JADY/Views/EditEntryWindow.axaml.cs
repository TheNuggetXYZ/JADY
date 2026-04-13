using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Backend;
using JADY.Models;

namespace JADY.Views;

public partial class EditEntryWindow : Window
{
    public EditEntryWindow()
    {
        InitializeComponent();

        EntryType.ItemsSource = Enum.GetValues(typeof(Utils.DiaryEntryType));
        EntryStatus.ItemsSource = Enum.GetValues(typeof(Utils.DiaryEntryStatus));
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
            Type = (Utils.DiaryEntryType)EntryType.SelectedIndex,
            Status = (Utils.DiaryEntryStatus)EntryStatus.SelectedIndex,
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
        
        if (EntryType.SelectedIndex == (int)Utils.DiaryEntryType.OneTime)
        {
            EntryStatus.SelectedIndex = (int)Utils.DiaryEntryStatus.None;
            EntryStatus.IsVisible = false;
        }
        else
        {
            if (EntryStatus.SelectedIndex == (int)Utils.DiaryEntryStatus.None)
            {
                EntryStatus.SelectedIndex = (int)Utils.DiaryEntryStatus.InProgress;
            }
            EntryStatus.IsVisible = true;
        }
    }

    private void EntryStatus_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (EntryType == null || EntryStatus == null)
            return;
        
        if (EntryStatus.SelectedIndex == (int)Utils.DiaryEntryStatus.None)
        {
            EntryType.SelectedIndex = (int)Utils.DiaryEntryType.OneTime;
        }
    }
}