using System;
using System.Collections.Generic;
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
    [ObservableProperty] private DateTime? _date;

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
    [ObservableProperty] private List<DiarySubEntryViewModel> _subEntries = new();

    public DiaryEntryViewModel(DiaryEntry diaryEntry)
    {
        Type = diaryEntry.Type;
        Status = diaryEntry.Status;
        LogDate = diaryEntry.LogDate;
        Date = diaryEntry.Date;
        EndDate = diaryEntry.EndDate;
        Category = diaryEntry.Category;
        SubCategory = diaryEntry.SubCategory;
        Title =  diaryEntry.Title;
        Content = diaryEntry.Content;
        SubEntries = InitializeSubEntries(diaryEntry.SubEntries);
    }
    
    private List<DiarySubEntryViewModel> InitializeSubEntries(List<DiarySubEntry> subEntryModels)
    {
        List<DiarySubEntryViewModel> subEntryViewModels = new();

        foreach (var subEntryModel in subEntryModels)
        {
            DiarySubEntryViewModel newDiarySubEntryViewModel = new(subEntryModel);
            subEntryViewModels.Add(newDiarySubEntryViewModel);
        }
        
        return subEntryViewModels;
    }
    
    public DiaryEntry GetModel()
    {
        return new()
        {
            Type = Type,
            Status = Status,
            LogDate = LogDate,
            Date = Date,
            EndDate = EndDate,
            Category = Category,
            SubCategory = SubCategory,
            Title = Title,
            Content = Content,
            SubEntries = DeinitializeSubEntries(SubEntries)
        };
    }
    
    private List<DiarySubEntry> DeinitializeSubEntries(List<DiarySubEntryViewModel> subEntryViewModels)
    {
        List<DiarySubEntry> subEntryModels = new();

        foreach (var subEntryViewModel in subEntryViewModels)
        {
            DiarySubEntry newDiarySubEntryModel = subEntryViewModel.GetModel();
            subEntryModels.Add(newDiarySubEntryModel);
        }
        
        return subEntryModels;
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