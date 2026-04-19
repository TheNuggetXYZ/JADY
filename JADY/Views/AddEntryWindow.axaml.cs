using System;
using Avalonia.Interactivity;
using JADY.Backend;
using JADY.Models;

namespace JADY.Views;

public partial class AddEntryWindow : DialogWindowBase<DiaryEntry>
{
    public AddEntryWindow()
    {
        InitializeComponent();

        EntryParameter.ItemsSource = Enum.GetValues(typeof(Utils.NewDiaryEntryParameter));
    }

    protected override DiaryEntry GetValue()
    {
        Utils.ConvertNewDiaryParameterToStatusAndType((Utils.NewDiaryEntryParameter)EntryParameter.SelectedIndex, out int status);

        return new DiaryEntry()
        {
            Status = (Utils.DiaryEntryStatus)status,
            Category = EntryCategory.Text,
            SubCategory = EntrySubcategory.Text,
            Title = EntryTitle.Text,
            Content = EntryContent.Text,
            LogDate = DateTimeOffset.Now,
            Date = EntryDate.SelectedDate,
            IsHidden = EntryIsHidden.IsChecked ?? false,
        };
    }

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
}