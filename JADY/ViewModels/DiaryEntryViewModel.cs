using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JADY.Core.Attributes;
using JADY.Core.Data;
using JADY.Core.Helpers;
using JADY.Core.Models;
using JADY.Services;
using JADY.UI.Views.Dialogs;

namespace JADY.ViewModels;

public partial class DiaryEntryViewModel : SaveDependentViewModel
{
    [NotifyPropertyChangedFor(nameof(ShowEndEventInContextMenu))]
    [NotifyPropertyChangedFor(nameof(ShowLinkInContextMenu))]
    [NotifyPropertyChangedFor(nameof(GetStatusDisplayName))]
    [ObservableProperty] private EntryStatus _status;

    /// <summary>
    /// The date at the time the entry was added.
    /// </summary>
    [NotifyPropertyChangedFor(nameof(LogDateDisplayName), nameof(FullDateTooltip), nameof(DisplayDate))]
    [ObservableProperty] private DateTimeOffset _logDate;

    /// <summary>
    /// The start date of an event or a date of a one time entry.
    /// </summary>
    [NotifyPropertyChangedFor(nameof(DateDisplayName), nameof(FullDateTooltip), nameof(DisplayDate))]
    [ObservableProperty] private DateTimeOffset? _date;

    /// <summary>
    /// The end date of an event. Is useless for a one time entry.
    /// </summary>
    [NotifyPropertyChangedFor(nameof(EndDateDisplayName), nameof(FullDateTooltip), nameof(DisplayDate))]
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
    [NotifyPropertyChangedFor(nameof(IsCurrentlyVisible))]
    [ObservableProperty] private bool _isHidden;
    
    /// <summary>
    /// The global unique identifier of this exact entry.
    /// </summary>
    public readonly Guid EntryGuid;
    
    /// <summary>
    /// The global unique identifier of the parent of this entry note.
    /// </summary>
    public Guid? ParentEntryGuid { get; private set; }

    public DiaryEntryViewModel? ParentEntry { get; private set; }
    
    [ObservableProperty] private bool _isExpanded;

    public string GetStatusDisplayName
    {
        get
        {
            return Status switch
            {
                EntryStatus.OneTime => "One time entry",
                EntryStatus.EventInProgress => "In progress event",
                EntryStatus.EventCompleted => "Completed event",
                EntryStatus.EventDropped => "Dropped event",
                EntryStatus.LinkNote => "Linked note",
                EntryStatus.LinkEndNote => "Linked end note",
                _ => throw new ArgumentOutOfRangeException(nameof(Status), Status, null)
            };
        }
    }

    public bool ShowEndEventInContextMenu => Status == EntryStatus.EventInProgress;
    public bool ShowLinkInContextMenu => Status == EntryStatus.EventInProgress;

    [SaveDependent]
    public bool IsCurrentlyVisible => !IsHidden || _saveService.Config.CurrentShowHiddenEntries;
    
    [SaveDependent]
    public string LogDateDisplayName => LogDate.Date.ToString("d", _saveService.Config.CultureInfo);

    [SaveDependent]
    public string? DateDisplayName => Date?.Date.ToString("d", _saveService.Config.CultureInfo);
    
    [SaveDependent]
    public string? EndDateDisplayName => EndDate?.Date.ToString("d", _saveService.Config.CultureInfo);

    [SaveDependent]
    public string DisplayDate => DateDisplayName ?? EndDateDisplayName ?? LogDateDisplayName;

    [SaveDependent]
    public string? FullDateTooltip =>
        $"Log date: {LogDateDisplayName}\n" +
        $"Date: {DateDisplayName ?? "none"}\n" +
        $"End date: {EndDateDisplayName ?? "none"}";
    
    
    private readonly DiaryViewModel _diaryViewModel;

    private readonly ISaveService _saveService;
    private readonly IWindowService _windowService;

    public DiaryEntryViewModel(DiaryEntry diaryEntry, DiaryViewModel diaryViewModel, DiaryEntryViewModel? parentEntry, ISaveService saveService, IWindowService windowService)
    {
        _saveService = saveService;
        _windowService = windowService;

        _diaryViewModel = diaryViewModel;
        Status = diaryEntry.Status;
        LogDate = diaryEntry.LogDate;
        Date = diaryEntry.Date;
        EndDate = diaryEntry.EndDate;
        Category = diaryEntry.Category;
        SubCategory = diaryEntry.SubCategory;
        Title =  diaryEntry.Title;
        Content = diaryEntry.Content;
        IsHidden = diaryEntry.IsHidden;
        EntryGuid = diaryEntry.EntryGuid;
        
        if (parentEntry is not null)
            AssignParentEntry(parentEntry);
        else
            ParentEntryGuid = diaryEntry.ParentEntryGuid;
    }
    
    /// <returns>
    /// a model using this view model's values.
    /// </returns>
    public DiaryEntry GetModel()
    {
        return new DiaryEntry
        {
            Status = Status,
            LogDate = LogDate,
            Date = Date,
            EndDate = EndDate,
            Category = Category,
            SubCategory = SubCategory,
            Title = Title,
            Content = Content,
            IsHidden = IsHidden,
            EntryGuid = EntryGuid,
            ParentEntryGuid = ParentEntryGuid,
        };
    }

    public void AssignParentEntry(DiaryEntryViewModel? parentEntry)
    {
        ParentEntryGuid = parentEntry?.EntryGuid;
        ParentEntry = parentEntry;
    }

    public void RestartEvent()
    {
        Status = EntryStatus.EventInProgress;
        EndDate = null;
    }
    
    [RelayCommand]
    private async Task ContextMenu_Remove()
    {
        await _diaryViewModel.RemoveEntry(this);
    }
    
    [RelayCommand]
    private async Task ContextMenu_Edit()
    {
        var diaryEntry = await _windowService.OpenDialogWindowDI<EditEntryWindow, DiaryEntry, DiaryEntryViewModel>(_windowService.GetMainWindow(), this);

        if (!diaryEntry.HasValue)
            return;
            
        Status = diaryEntry.Value.Status;
        Date = diaryEntry.Value.Date;
        EndDate = diaryEntry.Value.EndDate;
        Title = diaryEntry.Value.Title;
        Content = diaryEntry.Value.Content;
        IsHidden = diaryEntry.Value.IsHidden;

        if (Category != diaryEntry.Value.Category || SubCategory != diaryEntry.Value.SubCategory)
        {
            Category = diaryEntry.Value.Category;
            SubCategory = diaryEntry.Value.SubCategory;

            _diaryViewModel.CascadeEditEntries(this);
        }

        if (!EntryStatusExtensions.IsLink(diaryEntry.Value.Status))
            AssignParentEntry(null);

        _diaryViewModel.ResortEntry(this);
    }
    
    [RelayCommand]
    private async Task ContextMenu_Link()
    {
        var diaryEntry = await _windowService.OpenDialogWindowDI<LinkEntryWindow, DiaryEntry, DiaryEntryViewModel>(_windowService.GetMainWindow(), this);

        if (!diaryEntry.HasValue)
            return;

        if (diaryEntry.Value.Status == EntryStatus.LinkEndNote)
        {
            Status = EntryStatus.EventCompleted;
            EndDate = diaryEntry.Value.Date;
        }
        
        _diaryViewModel.AddEntry(diaryEntry.Value, this);
    }

    [RelayCommand]
    private async Task ContextMenu_End()
    {
        if (!ShowEndEventInContextMenu)
            return;
        
        var diaryEntry = await _windowService.OpenDialogWindowDI<EndEntryWindow, DiaryEntry, DiaryEntryViewModel>(_windowService.GetMainWindow(), this);
        
        if (!diaryEntry.HasValue)
            return;
        
        EndDate = diaryEntry.Value.EndDate;
        Status = diaryEntry.Value.Status;
        
        _diaryViewModel.ResortEntry(this);
    }
}
