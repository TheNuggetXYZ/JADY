using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Backend;
using JADY.Data;
using JADY.Models;
using JADY.ViewModels;

namespace JADY.Views;

public partial class EndEntryWindow : DialogWindow<DiaryEntry>, IDialogInitializable<DiaryEntryViewModel>
{
    public void Initialize(DiaryEntryViewModel data)
    {
        DataContext = data;
        InitializeComponent();
        
        EndParameter.ItemsSource = Utils.EndEntryParameterToArray;
    }

    protected override DiaryEntry GetValue()
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
    
    protected override InputElement? GetFirstFocusableElementOverride() => EndDate;

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
}