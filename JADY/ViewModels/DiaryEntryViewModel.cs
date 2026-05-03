using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Core;
using JADY.Core.Attributes;
using JADY.Core.Data;
using JADY.Core.Models;
using JADY.Services;
using EditEntryWindow = JADY.Views.Dialogs.EditEntryWindow;
using EndEntryWindow = JADY.Views.Dialogs.EndEntryWindow;

namespace JADY.ViewModels;

public partial class DiaryEntryViewModel : SaveDependentViewModel
{
    [NotifyPropertyChangedFor(nameof(ShowEndEventInContextMenu))]
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
    /// E.g. [I automated red science, I finally destroyed those biter nests, ...]
    /// </summary>
    public ObservableCollection<DiarySubEntryViewModel> SubEntries { get; set; }// DiaryEntryViewModel
    
    [ObservableProperty] private bool _isExpanded;

    public string GetStatusDisplayName
    {
        get
        {
            return Status switch
            {
                EntryStatus.OneTime => "One time event",
                EntryStatus.InProgress => "In progress event",
                EntryStatus.Completed => "Completed event",
                EntryStatus.Dropped => "Dropped event",
                _ => throw new ArgumentOutOfRangeException(nameof(Status), Status, null)
            };
        }
    }

    public bool ShowEndEventInContextMenu => Status == EntryStatus.InProgress;

    [SaveDependent]
    public bool IsCurrentlyVisible => !IsHidden || _saveService.JadySave.Settings.CurrentShowHiddenEntries;
    
    [SaveDependent]
    public string LogDateDisplayName => LogDate.Date.ToString("d", _saveService.JadySave.Settings.CultureInfo);

    [SaveDependent]
    public string? DateDisplayName => Date?.Date.ToString("d", _saveService.JadySave.Settings.CultureInfo);
    
    [SaveDependent]
    public string? EndDateDisplayName => EndDate?.Date.ToString("d", _saveService.JadySave.Settings.CultureInfo);

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

    public DiaryEntryViewModel(DiaryEntry diaryEntry, DiaryViewModel diaryViewModel, ISaveService saveService, IWindowService windowService)
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
        SubEntries = new(diaryEntry.SubEntries.Select(x => new DiarySubEntryViewModel(x, this)));
    }
    
    /// <returns>
    /// a model using this view model's values.
    /// </returns>
    public DiaryEntry GetModel()
    {
        return new()
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
            SubEntries = new(SubEntries.Select(x => x.GetModel()))
        };
    }
    
    [RelayCommand]
    private void ContextMenu_Remove()
    {
        _diaryViewModel.RemoveEntry(this);
    }
    
    [RelayCommand]
    private async Task ContextMenu_Edit()
    {
        Optional<DiaryEntry> diaryEntry = await _windowService.OpenDialogWindowDI<EditEntryWindow, DiaryEntry, DiaryEntryViewModel>(_windowService.GetMainWindow(), this);

        if (!diaryEntry.HasValue)
            return;
        
        Status = diaryEntry.Value.Status;
        Date = diaryEntry.Value.Date;
        EndDate = diaryEntry.Value.EndDate;
        Category = diaryEntry.Value.Category;
        SubCategory = diaryEntry.Value.SubCategory;
        Title = diaryEntry.Value.Title;
        Content = diaryEntry.Value.Content;
        IsHidden = diaryEntry.Value.IsHidden;

        _diaryViewModel.ResortEntry(this);
    }

    [RelayCommand]
    private async Task ContextMenu_End()
    {
        if (!ShowEndEventInContextMenu)
            return;
        
        Optional<DiaryEntry> diaryEntry = await _windowService.OpenDialogWindowDI<EndEntryWindow, DiaryEntry, DiaryEntryViewModel>(_windowService.GetMainWindow(), this);
        
        if (!diaryEntry.HasValue)
            return;
        
        EndDate = diaryEntry.Value.EndDate;
        Status = diaryEntry.Value.Status;
        
        _diaryViewModel.ResortEntry(this);
    }
    
    public void RemoveSubentry(DiarySubEntryViewModel item)
    {
        SubEntries.Remove(item);
        WeakReferenceMessenger.Default.Send(new Messages.UnsavedChangeCreated());
    }
}
