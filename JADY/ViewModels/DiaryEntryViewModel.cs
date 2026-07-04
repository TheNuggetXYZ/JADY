using System;
using System.Timers;
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

    public bool WasEndedByLinking;

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
    public bool IsCurrentlyVisible => !(IsHidden || (ParentEntry is not null && ParentEntry.IsHidden)) || _saveService.Config.CurrentShowHiddenEntries;
    
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
    
    private Timer? _highlightTimer;

    [ObservableProperty]
    private bool _isHighlighted;
    
    
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
        
        HighlightEntry();
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

    private void EndEvent(DateTimeOffset? endDate, EntryStatus status, bool endedByLinking)
    {
        Status = status;
        EndDate = endDate;
        WasEndedByLinking = endedByLinking;
    }
    
    [RelayCommand]
    private async Task ContextMenu_Remove()
    {
        await _diaryViewModel.RemoveEntry(this);
    }
    
    [RelayCommand]
    private async Task ContextMenu_Edit()
    {
        var newEntry = await _windowService.OpenDialogWindowDI<EditEntryWindow, DiaryEntry, DiaryEntryViewModel>(_windowService.GetMainWindow(), this);

        if (!newEntry.HasValue)
            return;

        // Update all children if category or subcategory was changed
        if (Category != newEntry.Value.Category || SubCategory != newEntry.Value.SubCategory)
        {
            Category = newEntry.Value.Category;
            SubCategory = newEntry.Value.SubCategory;
            
            _diaryViewModel.CascadeEditEntries(this);
        }

        // Restart parent if changed from LinkEndNote
        if (Status == EntryStatus.LinkEndNote && newEntry.Value.Status != Status) 
            ParentEntry?.RestartEvent();

        // End parent if edited to LinkEndNote
        if (newEntry.Value.Status == EntryStatus.LinkEndNote && Status != EntryStatus.LinkEndNote)
            ParentEntry?.EndEvent(newEntry.Value.Date, EntryStatus.EventCompleted, true);

        // Update parent end date if we edited the date of an end note
        if (Date != newEntry.Value.Date && newEntry.Value.Status == EntryStatus.LinkEndNote)
            ParentEntry?.EndDate = newEntry.Value.Date;
            
        Date = newEntry.Value.Date;
        EndDate = newEntry.Value.EndDate;
        Title = newEntry.Value.Title;
        Content = newEntry.Value.Content;
        IsHidden = newEntry.Value.IsHidden;
        Status = newEntry.Value.Status;

        if (!EntryStatusExtensions.IsLink(newEntry.Value.Status))
            AssignParentEntry(null);

        _diaryViewModel.ResortEntry(this);
        
        HighlightEntry();
    }
    
    [RelayCommand]
    private async Task ContextMenu_Link()
    {
        await Link(false);
    }
    
    [RelayCommand]
    private async Task ContextMenu_LinkEnd()
    {
        await Link(true);
    }

    private async Task Link(bool endOnly)
    {
        var diaryEntry =
            await _windowService.OpenDialogWindowDI<LinkEntryWindow, DiaryEntry, LinkEntryInitData>(
                _windowService.GetMainWindow(), new() { Entry = this, DefaultToEndNote = endOnly });

        if (!diaryEntry.HasValue)
            return;

        if (diaryEntry.Value.Status == EntryStatus.LinkEndNote) 
            EndEvent(diaryEntry.Value.Date, EntryStatus.EventCompleted, true);
        
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
        
        EndEvent(diaryEntry.Value.EndDate, diaryEntry.Value.Status, false);
        
        _diaryViewModel.ResortEntry(this);
        
        HighlightEntry();
    }
    
    [RelayCommand]
    private void OnClick()
    {
        IsExpanded = !IsExpanded;
    }

    private void HighlightEntry()
    {
        IsHighlighted = true;

        _highlightTimer?.Stop();
        _highlightTimer?.Dispose();

        _highlightTimer = new Timer(400);
        _highlightTimer.AutoReset = false; // Only run once
        
        _highlightTimer.Elapsed += (_, _) =>
        {
            IsHighlighted = false;
        };

        _highlightTimer.Start();
    }
}
