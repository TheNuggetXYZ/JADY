using System;
using CommunityToolkit.Mvvm.ComponentModel;
using JADY.Models;

namespace JADY.ViewModels;

public partial class DiaryEntryViewModel : ViewModelBase
{
    [ObservableProperty] private DiaryEntryType _type;

    [ObservableProperty] private DiaryEntryStatus _status;

    /// <summary>
    /// The date at the time the entry was added.
    /// </summary>
    [ObservableProperty] private DateTime _logDate;

    /// <summary>
    /// The start date of an event or a date of a one time entry.
    /// </summary>
    [ObservableProperty] private DateTime _date;

    /// <summary>
    /// The end date of an event. Is useless for a one time entry.
    /// </summary>
    [ObservableProperty] private DateTime? _endDate;

    /// <summary>
    /// E.g. Game/Anime/Misc
    /// </summary>
    [ObservableProperty] private string? _category;
    
    /// <summary>
    /// E.g. Factorio/OPM/null
    /// </summary>
    [ObservableProperty] private string? _subCategory;

    /// <summary>
    /// E.g. First time playing Factorio
    /// </summary>
    [ObservableProperty] private string? _title;

    /// <summary>
    /// E.g. I started playing Factorio for the first time on the ... version and its ...
    /// </summary>
    [ObservableProperty] private string? _content;

    /// <summary>
    /// E.g. [I automated red science, I finally destroyed those biter nests, ...]
    /// </summary>
    [ObservableProperty] private DiarySubEntry[]? _subEntries;

    public DiaryEntryViewModel(DiaryEntry diaryEntry)
    {
        Type = diaryEntry.Type;
        Status = diaryEntry.Status;
        LogDate = diaryEntry.LogDate;
        Date = diaryEntry.Date;
        EndDate = diaryEntry.EndDate;
        Category = diaryEntry.Category;
        Title =  diaryEntry.Title;
        Content = diaryEntry.Content;
        SubEntries = diaryEntry.SubEntries;
    }

    public string GetStatusDisplayName()
    {
        return Status switch
        {
            DiaryEntryStatus.None => "None",
            DiaryEntryStatus.InProgress => "In progress",
            DiaryEntryStatus.Completed => "Completed",
            DiaryEntryStatus.Dropped => "Dropped",
            _ => throw new ArgumentOutOfRangeException(nameof(Status), Status, null)
        };
    }

    public string GetTypeDisplayName()
    {
        return Type switch
        {
            DiaryEntryType.OneTime => "One time",
            DiaryEntryType.ProlongedEvent => "Event",
            _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
        };
    }

    
    public string SubCategoryStringWithPipeSeparator
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(SubCategory))
                return " | " + SubCategory;

            return string.Empty;
        }
    }
}