using System;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Core.Data;
using JADY.Core.Models;
using JADY.Views.Base;

namespace JADY.Views.Dialogs;

public partial class AddEntryWindow : DialogWindow<DiaryEntry>
{
    public AddEntryWindow()
    {
        InitializeComponent();

        EntryParameter.ItemsSource = new[]{"One time", "Started"};
    }

    protected override Optional<DiaryEntry> GetValue()
    {
        return new DiaryEntry()
        {
            Category = EntryCategory.Text,
            SubCategory = EntrySubcategory.Text,
            Title = EntryTitle.Text,
            Content = EntryContent.Text,
            LogDate = DateTimeOffset.Now,
            Date = EntryDate.SelectedDate,
            IsHidden = EntryIsHidden.IsChecked ?? false,
            Status = (NewEntryParameter)EntryParameter.SelectedIndex switch
            {
                NewEntryParameter.OneTime => EntryStatus.OneTime,
                NewEntryParameter.Started => EntryStatus.InProgress,
                _ => throw new ArgumentOutOfRangeException(nameof(EntryParameter), EntryParameter, null)
            },
        };
    }
    
    protected override InputElement? FocusedElement() => EntryCategory;

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
}