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

        EntryStatus.ItemsSource = Enum.GetValues(typeof(Utils.EntryStatus));
    }

    protected override DiaryEntry GetValue()
    {
        return new DiaryEntry()
        {
            Status = (Utils.EntryStatus)EntryStatus.SelectedIndex,
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
}