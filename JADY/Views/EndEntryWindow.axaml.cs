using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Backend;
using JADY.Models;

namespace JADY.Views;

public partial class EndEntryWindow : DialogWindowBase<DiaryEntry>
{
    public EndEntryWindow()
    {
        InitializeComponent();
        
        EndParameter.ItemsSource = Utils.EndEntryParameterToArray;
    }

    protected override DiaryEntry GetValue()
    {
        return new DiaryEntry()
        {
            EndDate = EndDate.SelectedDate,
            Status = (Utils.EndEntryParameter)EndParameter.SelectedIndex switch
            {
                Utils.EndEntryParameter.Completed => Utils.EntryStatus.Completed,
                Utils.EndEntryParameter.Dropped => Utils.EntryStatus.Dropped,
                _ => throw new ArgumentOutOfRangeException()
            }
        };
    }
    
    protected override InputElement? GetFirstFocusableElementOverride() => EndDate;

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
}