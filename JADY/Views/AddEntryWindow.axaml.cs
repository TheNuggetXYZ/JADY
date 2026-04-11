using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using JADY.Backend;
using JADY.Models;
using JADY.ViewModels;

namespace JADY.Views;

public partial class AddEntryWindow : Window
{
    public AddEntryWindow()
    {
        InitializeComponent();

        EntryParameter.ItemsSource = Enum.GetValues(typeof(NewDiaryEntryParameter));
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
        Utils.ConvertNewDiaryParameterToStatusAndType((NewDiaryEntryParameter)EntryParameter.SelectedIndex, out int status, out int type);
        
        Close(new DiaryEntry()
        {
            Status = (DiaryEntryStatus)status,
            Type = (DiaryEntryType)type,
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