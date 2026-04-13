using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JADY.Models;

namespace JADY.ViewModels;

public partial class DiarySubEntryViewModel : ViewModelBase
{
    /// <summary>
    /// The date at the time the sub entry was added.
    /// </summary>
    [ObservableProperty] private DateTimeOffset _logDate;

    /// <summary>
    /// The date of a sub entry.
    /// </summary>
    [ObservableProperty] private DateTimeOffset _date;

    /// <summary>
    /// E.g.:
    /// I automated red science.
    /// I finnaly destroyed those biter nests.
    /// </summary>
    [ObservableProperty] private string? _content;
    
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    private readonly DiaryEntryViewModel _diaryEntryViewModel;

    public DiarySubEntryViewModel(DiarySubEntry subEntry, DiaryEntryViewModel diaryEntryViewModel)
    {
        _diaryEntryViewModel = diaryEntryViewModel;
        LogDate = subEntry.LogDate;
        Date = subEntry.Date;
        Content = subEntry.Content;
    }

    /// <returns>
    /// a model using this view model's values.
    /// </returns>
    public DiarySubEntry GetModel()
    {
        return new DiarySubEntry()
        {
            LogDate = LogDate,
            Date = Date,
            Content = Content
        };
    }
    
    [RelayCommand]
    private void ContextMenu_Remove()
    {
        _diaryEntryViewModel.RemoveMyself(this);
    }
}