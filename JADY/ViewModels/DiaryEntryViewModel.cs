using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    [ObservableProperty] private DateTimeOffset _logDate;

    /// <summary>
    /// The start date of an event or a date of a one time entry.
    /// </summary>
    [ObservableProperty] private DateTimeOffset? _date;

    /// <summary>
    /// The end date of an event. Is useless for a one time entry.
    /// </summary>
    [ObservableProperty] private DateTimeOffset? _endDate;

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
    /// Used if you don't want this entry to appear unless you toggle on a setting to show hidden entries.
    /// </summary>
    [ObservableProperty] private bool _isHidden;

    /// <summary>
    /// E.g. [I automated red science, I finally destroyed those biter nests, ...]
    /// </summary>
    public ObservableCollection<DiarySubEntryViewModel> SubEntries { get; set; }

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
        IsHidden = diaryEntry.IsHidden;
        SubEntries = InitializeSubEntries(diaryEntry.SubEntries);
    }
    
    /// <summary>
    /// Converts a list of models to a list of their corresponding view models using the view model's constructor.
    /// </summary>
    private ObservableCollection<DiarySubEntryViewModel> InitializeSubEntries(List<DiarySubEntry> subEntryModels)
    {
        ObservableCollection<DiarySubEntryViewModel> subEntryViewModels = new();

        foreach (var subEntryModel in subEntryModels)
        {
            DiarySubEntryViewModel newDiarySubEntryViewModel = new(subEntryModel);
            subEntryViewModels.Add(newDiarySubEntryViewModel);
        }
        
        return subEntryViewModels;
    }
    
    /// <returns>
    /// a model using this view model's values.
    /// </returns>
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
    
    /// <summary>
    /// Converts a list of view models to a list of their corresponding models using the view model's GetModel method.
    /// </summary>
    private List<DiarySubEntry> DeinitializeSubEntries(ObservableCollection<DiarySubEntryViewModel> subEntryViewModels)
    {
        List<DiarySubEntry> subEntryModels = new();

        foreach (var subEntryViewModel in subEntryViewModels)
        {
            DiarySubEntry newDiarySubEntryModel = subEntryViewModel.GetModel();
            subEntryModels.Add(newDiarySubEntryModel);
        }
        
        return subEntryModels;
    }

    public string GetStatusDisplayNameWithSpace
    {
        get
        {
            return Status switch
            {
                DiaryEntryStatus.None => "",
                DiaryEntryStatus.InProgress => "In progress ",
                DiaryEntryStatus.Completed => "Completed ",
                DiaryEntryStatus.Dropped => "Dropped ",
                _ => throw new ArgumentOutOfRangeException(nameof(Status), Status, null)
            };
        }
    }

    public string GetTypeDisplayName
    {
        get
        {
            return Type switch
            {
                DiaryEntryType.OneTime => "One time",
                DiaryEntryType.ProlongedEvent => "event",
                _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
            };
        }
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