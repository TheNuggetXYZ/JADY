using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Backend;
using JADY.Models;

namespace JADY.Views;

public partial class AddEntryWindow : Window
{
    public AddEntryWindow()
    {
        InitializeComponent();

        EntryParameter.ItemsSource = Enum.GetValues(typeof(Utils.NewDiaryEntryParameter));
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
        Utils.ConvertNewDiaryParameterToStatusAndType((Utils.NewDiaryEntryParameter)EntryParameter.SelectedIndex, out int status, out int type);
        
        Close(new DiaryEntry()
        {
            Status = (Utils.DiaryEntryStatus)status,
            Type = (Utils.DiaryEntryType)type,
            Category = EntryCategory.Text,
            SubCategory = EntrySubcategory.Text,
            Title = EntryTitle.Text,
            Content = EntryContent.Text,
            LogDate = DateTimeOffset.Now,
            Date = EntryDate.SelectedDate,
            IsHidden = EntryIsHidden.IsChecked ?? false,
        });
    }
}