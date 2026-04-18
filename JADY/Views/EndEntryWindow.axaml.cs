using System;
using Avalonia.Interactivity;
using JADY.Backend;
using JADY.Models;

namespace JADY.Views;

public partial class EndEntryWindow : DialogWindowBase<DiaryEntry>
{
    public EndEntryWindow()
    {
        InitializeComponent();
        
        EndParameter.ItemsSource = Enum.GetValues(typeof(Utils.EndDiaryParameter));
    }

    protected override DiaryEntry GetValue()
    {
        return new DiaryEntry()
        {
            EndDate = EndDate.SelectedDate,
            Status = (Utils.EndDiaryParameter)EndParameter.SelectedIndex switch
            {
                Utils.EndDiaryParameter.Finished => Utils.DiaryEntryStatus.Completed,
                Utils.EndDiaryParameter.Dropped => Utils.DiaryEntryStatus.Dropped,
                _ => throw new ArgumentOutOfRangeException()
            }
        };
    }

    private async void Submit_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync();
}