using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.JavaScript;
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

    [NotifyPropertyChangedFor(nameof(ShowEndEventInContextMenu))]
    [NotifyPropertyChangedFor(nameof(GetStatusDisplayNameWithSpace))]
    [ObservableProperty] private DiaryEntryStatus _status;

    /// <summary>
    /// The date at the time the entry was added.
    /// </summary>
    [NotifyPropertyChangedFor(nameof(LogDateDisplayName))]
    [ObservableProperty] private DateTimeOffset _logDate;

    /// <summary>
    /// The start date of an event or a date of a one time entry.
    /// </summary>
    [NotifyPropertyChangedFor(nameof(DateDisplayName))]
    [ObservableProperty] private DateTimeOffset? _date;

    /// <summary>
    /// The end date of an event. Is useless for a one time entry.
    /// </summary>
    [NotifyPropertyChangedFor(nameof(EndDateDisplayName))]
    [ObservableProperty] private DateTimeOffset? _endDate;

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
    [NotifyPropertyChangedFor(nameof(IsCurrentlyVisible))]
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

    public bool ShowEndEventInContextMenu => Type == DiaryEntryType.ProlongedEvent && Status == DiaryEntryStatus.InProgress;

    public bool IsCurrentlyVisible => !IsHidden || DiaryJSON.JadySave.Settings.ShowHiddenEntries;
    
    public string LogDateDisplayName => LogDate.Date.ToShortDateString();

    public string? DateDisplayName => Date?.Date.ToShortDateString();
    
    public string? EndDateDisplayName => EndDate?.Date.ToShortDateString();

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

        DiaryJSON.OnSaveChanged += () => { OnPropertyChanged(nameof(IsCurrentlyVisible)); };
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
    private void ContextMenu_Remove()
    {
        _diaryViewModel.RemoveMyself(this);
    }
    
    public void RemoveMyself(DiarySubEntryViewModel item)
    {
        SubEntries.Remove(item);
    }

    [RelayCommand]
    private async Task ContextMenu_End()
    {
        if (!ShowEndEventInContextMenu)
            return;
        
        DiaryEntry diaryEntry = await WindowManager.OpenDialogWindow<EndEntryWindow, DiaryEntry>(WindowManager.GetMainWindow(), this);
        
        if (diaryEntry == null)
            return;
        
        if (Type != DiaryEntryType.ProlongedEvent)
            return;
        
        EndDate = diaryEntry.EndDate;
        Status = diaryEntry.Status;
    }

    [RelayCommand]
    private async Task ContextMenu_Edit()
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