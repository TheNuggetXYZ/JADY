using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Backend;
using JADY.Data;
using JADY.Models;

namespace JADY.Views;

public partial class AddEntryWindow : DialogWindow<DiaryEntry>
{
    public AddEntryWindow()
    {
        InitializeComponent();

        EntryParameter.ItemsSource = Utils.NewEntryParameterToArray;
    }

    protected override DiaryEntry GetValue()
    {
        return new DiaryEntry()
        {
            Status = Utils.NewEntryParameterToEntryStatus((NewEntryParameter)EntryParameter.SelectedIndex),
            Category = EntryCategory.Text,
            SubCategory = EntrySubcategory.Text,
            Title = EntryTitle.Text,
            Content = EntryContent.Text,
            LogDate = DateTimeOffset.Now,
            Date = EntryDate.SelectedDate,
            IsHidden = EntryIsHidden.IsChecked ?? false,
        };
    }
    
    protected override InputElement? GetFirstFocusableElementOverride() => EntryCategory;

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
}