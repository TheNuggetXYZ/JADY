using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
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