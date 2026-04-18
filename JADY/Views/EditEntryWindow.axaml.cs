using System;
using Avalonia;
using Avalonia.Interactivity;
using JADY.Backend;
using JADY.Models;

namespace JADY.Views;

public partial class EditEntryWindow : DialogWindowBase<DiaryEntry>
{
    public EditEntryWindow()
    {
        InitializeComponent();

        EntryType.ItemsSource = Enum.GetValues(typeof(Utils.DiaryEntryType));
        EntryStatus.ItemsSource = Enum.GetValues(typeof(Utils.DiaryEntryStatus));
    }

    protected override DiaryEntry GetValue()
    {
        return new DiaryEntry()
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
        };
    }

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();

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