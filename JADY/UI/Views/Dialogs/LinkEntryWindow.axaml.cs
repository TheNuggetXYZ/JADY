using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using JADY.Core.Data;
using JADY.Core.Models;
using JADY.UI.Base;
using JADY.ViewModels;

namespace JADY.UI.Views.Dialogs;

public partial class LinkEntryWindow : DialogWindow<DiaryEntry>, IDialogInitializable<DiaryEntryViewModel>
{
    public void Initialize(DiaryEntryViewModel data)
    {
        DataContext = data;
        
        InitializeComponent();
        
        EntryStatus.ItemsSource = new[] {"Note", "End note"};
    }

    protected override Optional<DiaryEntry> GetValue()
    {
        return new DiaryEntry
        {
            Category = EntryCategory.Text,
            SubCategory = EntrySubcategory.Text,
            Title = EntryTitle.Text,
            Content = EntryContent.Text,
            LogDate = DateTimeOffset.Now,
            Date = EntryDate.SelectedDate,
            IsHidden = EntryIsHidden.IsChecked ?? false,
            Status = (LinkEntryParameter)EntryStatus.SelectedIndex switch
            {
                LinkEntryParameter.Note => Core.Data.EntryStatus.Note,
                LinkEntryParameter.EndNote => Core.Data.EntryStatus.EndNote,
                _ => throw new ArgumentOutOfRangeException()
            }
        };
    }

    protected override InputElement? FocusedElement() => EntryTitle;

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
}