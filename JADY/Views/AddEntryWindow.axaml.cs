using System;
using Avalonia;
using Avalonia.Controls;
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

    private void Submit_OnClick(object? sender, RoutedEventArgs e)
    {
        Utils.ConvertNewDiaryParameterToStatusAndType((NewDiaryEntryParameter)EntryParameter.SelectedIndex, out int status, out int type);
        
        Close(new Models.DiaryEntry()
        {
            Status = (DiaryEntryStatus)status,
            Type = (DiaryEntryType)type,
            Category = EntryCategory.Text,
            SubCategory = EntrySubcategory.Text,
            Title = EntryTitle.Text,
            LogDate = DateTimeOffset.Now,
            Date = EntryDate.SelectedDate,
            IsHidden = EntryIsHidden.IsChecked ?? false,
        });
    }
}