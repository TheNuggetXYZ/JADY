using System;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Backend;
using JADY.Core.Data;
using JADY.Core.Models;
using JADY.ViewModels;
using JADY.Views.Base;

namespace JADY.Views.Dialogs;

public partial class EndEntryWindow : DialogWindow<DiaryEntry>, IDialogInitializable<DiaryEntryViewModel>
{
    public void Initialize(DiaryEntryViewModel data)
    {
        DataContext = data;
        InitializeComponent();
        
        EndParameter.ItemsSource = new[]{"Completed", "Dropped"};
    }

    protected override Optional<DiaryEntry> GetValue()
    {
        return new DiaryEntry()
        {
            EndDate = EndDate.SelectedDate,
            Status = (EndEntryParameter)EndParameter.SelectedIndex switch
            {
                EndEntryParameter.Completed => EntryStatus.Completed,
                EndEntryParameter.Dropped => EntryStatus.Dropped,
                _ => throw new ArgumentOutOfRangeException()
            }
        };
    }
    
    protected override InputElement? FocusedElement() => EndDate;

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
}