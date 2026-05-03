using System;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Backend;
using JADY.Core.Data;
using JADY.Core.Models;
using JADY.Views.Base;

namespace JADY.Views.Dialogs;

public partial class AddEntryWindow : DialogWindow<DiaryEntry>
{
    public AddEntryWindow()
    {
        InitializeComponent();

        EntryParameter.ItemsSource = Utils.NewEntryParameterToArray;
    }

    protected override Optional<DiaryEntry> GetValue()
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
    
    protected override InputElement? FocusedElement() => EntryCategory;

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
}