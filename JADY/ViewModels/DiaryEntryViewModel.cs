using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JADY.Backend;
using JADY.Models;
using JADY.Views;
using DiaryEntry = JADY.Models.DiaryEntry;

namespace JADY.ViewModels;

public partial class DiaryEntryViewModel : ViewModelBase
{
    [NotifyPropertyChangedFor(nameof(ShowEndEventInContextMenu))]
    [NotifyPropertyChangedFor(nameof(GetTypeDisplayName))]
    [ObservableProperty] private DiaryEntryType _type;

    [NotifyPropertyChangedFor(nameof(GetStatusDisplayNameWithSpace))]
    [ObservableProperty] private DiaryEntryStatus _status;
    [ObservableProperty] private EndDiaryParameter _newEndParameter;
    public Array NewEndParameterValues => Enum.GetValues(typeof(EndDiaryParameter));

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
    [ObservableProperty] private DateTimeOffset? _newEndDate;

    /// <summary>
    /// E.g. Game/Anime/Misc
    /// </summary>
    [ObservableProperty] private string? _category;
    
    /// <summary>
    /// E.g. Factorio/OPM/null
    /// </summary>
    [NotifyPropertyChangedFor(nameof(SubCategoryStringWithPipeSeparator))]
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

    public bool ShowEndEventInContextMenu => Type == DiaryEntryType.ProlongedEvent;

    private readonly MainWindowViewModel _mainWindowViewModel;
    
    private readonly DiaryViewModel _diaryViewModel;

    public DiaryEntryViewModel(DiaryEntry diaryEntry, MainWindowViewModel mainWindowViewModel, DiaryViewModel diaryViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _diaryViewModel = diaryViewModel;
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
            IsHidden = IsHidden,
            SubEntries = DeinitializeSubEntries(SubEntries)
        };
    }
    
    /// <summary>
    /// Converts a list of models to a list of their corresponding view models using the view model's constructor.
    /// </summary>
    private ObservableCollection<DiarySubEntryViewModel> InitializeSubEntries(List<DiarySubEntry> subEntryModels)
    {
        ObservableCollection<DiarySubEntryViewModel> subEntryViewModels = new();

        foreach (var subEntryModel in subEntryModels)
        {
            DiarySubEntryViewModel newDiarySubEntryViewModel = new(subEntryModel, _mainWindowViewModel, this);
            subEntryViewModels.Add(newDiarySubEntryViewModel);
        }
        
        return subEntryViewModels;
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
    
    [RelayCommand]
    private void Remove()
    {
        _diaryViewModel.RemoveMyself(this);
    }
    
    public void RemoveMyself(DiarySubEntryViewModel item)
    {
        SubEntries.Remove(item);
    }

    [RelayCommand]
    private void End()
    {
        if (Type != DiaryEntryType.ProlongedEvent)
            return;
        
        EndDate = NewEndDate;
        Status = NewEndParameter switch
        {
            EndDiaryParameter.Finished => DiaryEntryStatus.Completed,
            EndDiaryParameter.Dropped => DiaryEntryStatus.Dropped,
            _ => Status
        };
        
        WindowManager.CloseWindow<EndEntryWindow>();
    }

    [RelayCommand]
    private void OpenEndWindow()
    {
        WindowManager.OpenDialogWindow<EndEntryWindow, object?>(WindowManager.GetMainWindow(), this);
    }

    [RelayCommand]
    private async Task Edit()
    {
        DiaryEntry diaryEntry = await WindowManager.OpenDialogWindow<EditEntryWindow, DiaryEntry>(WindowManager.GetMainWindow(), this);

        if (diaryEntry == null)
            return;
        
        Type = diaryEntry.Type;
        Status = diaryEntry.Status;
        Date = diaryEntry.Date;
        EndDate = diaryEntry.EndDate;
        Category = diaryEntry.Category;
        SubCategory = diaryEntry.SubCategory;
        Title = diaryEntry.Title;
        Content = diaryEntry.Content;
        IsHidden = diaryEntry.IsHidden;
    }
}

public enum EndDiaryParameter
{
    Finished = 0,
    Dropped = 1,
}