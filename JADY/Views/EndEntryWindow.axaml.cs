using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Models;
using JADY.ViewModels;

namespace JADY.Views;

public partial class EndEntryWindow : Window
{
    public EndEntryWindow()
    {
        InitializeComponent();
        
        EndParameter.ItemsSource = Enum.GetValues(typeof(EndDiaryParameter));
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
        Close(new DiaryEntry()
        {
            EndDate = EndDate.SelectedDate,
            Status = (EndDiaryParameter)EndParameter.SelectedIndex switch
            {
                EndDiaryParameter.Finished => DiaryEntryStatus.Completed,
                EndDiaryParameter.Dropped => DiaryEntryStatus.Dropped,
                _ => throw new ArgumentOutOfRangeException()
            }
        });
    }
}