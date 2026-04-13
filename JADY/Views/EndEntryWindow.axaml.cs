using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Backend;
using JADY.Models;

namespace JADY.Views;

public partial class EndEntryWindow : Window
{
    public EndEntryWindow()
    {
        InitializeComponent();
        
        EndParameter.ItemsSource = Enum.GetValues(typeof(Utils.EndDiaryParameter));
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
            Status = (Utils.EndDiaryParameter)EndParameter.SelectedIndex switch
            {
                Utils.EndDiaryParameter.Finished => Utils.DiaryEntryStatus.Completed,
                Utils.EndDiaryParameter.Dropped => Utils.DiaryEntryStatus.Dropped,
                _ => throw new ArgumentOutOfRangeException()
            }
        });
    }
}